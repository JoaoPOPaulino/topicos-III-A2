
using A2.Data;
using A2.DTOs;
using A2.Models;
using A2.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2.Services
{
    public class PrestacaoContasService : IPrestacaoContasService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<PrestacaoContasService> _logger;

        private const int MoedaBRLId = 1;

        public PrestacaoContasService(
            ApplicationDbContext context,
            IExchangeRateService exchangeRateService,
            ILogger<PrestacaoContasService> logger)
        {
            _context = context;
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        public async Task<PrestacaoContas> CreateAsync(PrestacaoContasCreateDto dto, int criadoPorId)
        {
            var adiantamento = await _context.SolicitacoesAdiantamento
                .FirstOrDefaultAsync(a => a.Id == dto.SolicitacaoAdiantamentoId);

            if (adiantamento == null)
                throw new KeyNotFoundException($"Solicitação de Adiantamento ID {dto.SolicitacaoAdiantamentoId} não encontrada.");

            if (adiantamento.Status != StatusAdiantamento.Pago && adiantamento.Status != StatusAdiantamento.PrestacaoPendente)
                throw new InvalidOperationException($"A prestação só pode ser criada para adiantamentos em status PAGO ou PRESTACAO PENDENTE. Status atual: {adiantamento.Status}.");

            var existingReport = await _context.PrestacoesContas
                .AnyAsync(p => p.SolicitacaoAdiantamentoId == dto.SolicitacaoAdiantamentoId && p.Status != StatusPrestacao.Finalizada);

            if (existingReport)
                throw new InvalidOperationException("Já existe uma prestação de contas ativa para este adiantamento.");


            var prestacao = new PrestacaoContas
            {
                SolicitacaoAdiantamentoId = dto.SolicitacaoAdiantamentoId,
                CriadoPorId = criadoPorId,
                Status = StatusPrestacao.Enviada,
                EnviadoEm = DateTime.UtcNow,
                Despesas = new List<Despesa>()
            };

            decimal totalDespesasBRL = 0;

            foreach (var despesaDto in dto.Despesas)
            {
                decimal taxaCambio = 1.0m;
                decimal valorEmBRL = despesaDto.Valor;

                if (despesaDto.MoedaId != MoedaBRLId)
                {
                    try
                    {

                        taxaCambio = await _exchangeRateService.GetRateAsync(MoedaBRLId, despesaDto.MoedaId, despesaDto.DataDespesa);

                        valorEmBRL = despesaDto.Valor * taxaCambio;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Falha na conversão da despesa em moeda estrangeira (Moeda ID: {despesaDto.MoedaId}).");
                        throw new InvalidOperationException($"Não foi possível obter a cotação na data {despesaDto.DataDespesa.ToShortDateString()}. Motivo: {ex.Message}");
                    }
                }

                totalDespesasBRL += valorEmBRL;

                var despesa = new Despesa
                {
                    PrestacaoContas = prestacao,
                    CategoriaId = despesaDto.CategoriaId,
                    FornecedorId = despesaDto.FornecedorId,
                    MoedaId = despesaDto.MoedaId,
                    Descricao = despesaDto.Descricao,
                    DataDespesa = despesaDto.DataDespesa.Date,
                    Valor = despesaDto.Valor,
                    TaxaCambio = taxaCambio,
                    ValorEmBrl = valorEmBRL,
                    Observacoes = despesaDto.Observacoes
                };

                prestacao.Despesas.Add(despesa);
            }

            prestacao.TotalDespesas = totalDespesasBRL;
            decimal adiantamentoValor = adiantamento.Valor;

            if (totalDespesasBRL > adiantamentoValor)
            {
                prestacao.SaldoReembolso = totalDespesasBRL - adiantamentoValor;
                prestacao.SaldoDevolucao = 0;
            }
            else if (totalDespesasBRL < adiantamentoValor)
            {

                prestacao.SaldoReembolso = 0;
                prestacao.SaldoDevolucao = adiantamentoValor - totalDespesasBRL;
            }
            else
            {
                prestacao.SaldoReembolso = 0;
                prestacao.SaldoDevolucao = 0;
            }

            _context.PrestacoesContas.Add(prestacao);

            adiantamento.Status = StatusAdiantamento.PrestacaoEnviada;

            await _context.SaveChangesAsync();

            await LogAuditoriaAsync(criadoPorId, "PrestacaoContas", prestacao.Id, "CREATE_REPORT_AND_SUBMIT");

            return prestacao;
        }

        private async Task LogAuditoriaAsync(int usuarioId, string entidade, int entidadeId, string acao)
        {
            var log = new LogAuditoria
            {
                UsuarioId = usuarioId,
                TipoEntidade = entidade,
                EntidadeId = entidadeId,
                Acao = acao,
                CriadoEm = DateTime.UtcNow
            };

            _context.LogsAuditoria.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}