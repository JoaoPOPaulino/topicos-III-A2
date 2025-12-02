using A2.Data;
using A2.DTOs;
using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models;
using A2.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A2.Services
{
    public class SolicitacaoAdiantamentoService : ISolicitacaoAdiantamentoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHolidayService _holidayService;
        private readonly ILogger<SolicitacaoAdiantamentoService> _logger;

        public SolicitacaoAdiantamentoService(
            ApplicationDbContext context,
            IHolidayService holidayService,
            ILogger<SolicitacaoAdiantamentoService> logger)
        {
            _context = context;
            _holidayService = holidayService;
            _logger = logger;
        }

        // ---------------------------------------------------------------------
        // C - CREATE
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamento> CreateAsync(SolicitacaoAdiantamentoCreateDto dto, int criadoPorId)
        {
            _logger.LogInformation("Service: Tentativa de criação de adiantamento para Colab ID {ColabId}.", dto.ColaboradorId);

            // Validações...
            var colaboradorExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.ColaboradorId && u.Ativo);

            if (!colaboradorExiste)
            {
                _logger.LogWarning("Colaborador ID {Id} não encontrado.", dto.ColaboradorId);
                throw new InvalidOperationException("Colaborador não encontrado ou inativo.");
            }

            // Validação de pendência (Simplificada)
            var temAdiantamentoPendente = await _context.SolicitacoesAdiantamento
                .AnyAsync(s => s.ColaboradorId == dto.ColaboradorId
                             && (s.Status == StatusAdiantamento.Pendente
                             || s.Status == StatusAdiantamento.Aprovado
                             || s.Status == StatusAdiantamento.PrestacaoPendente));

            if (temAdiantamentoPendente)
            {
                _logger.LogWarning("Criação falhou: Colaborador já possui adiantamento pendente.");
                throw new InvalidOperationException("Colaborador possui adiantamento pendente (Pendente, Aprovado ou Prestação Pendente).");
            }

            if (dto.DataPagamentoRequerida.Date < DateTime.Today)
                throw new InvalidOperationException("Data de pagamento não pode ser no passado.");

            // AÇÃO DE NEGÓCIO: Ajustar Data de Pagamento para o próximo dia útil
            var dataAjustada = await _holidayService.GetNextBusinessDayAsync(dto.DataPagamentoRequerida);
            _logger.LogInformation("Data Requerida: {Req:d}, Data Ajustada (Feriados): {Adj:d}",
                                    dto.DataPagamentoRequerida.Date, dataAjustada.Date);

            var solicitacao = new SolicitacaoAdiantamento
            {
                ColaboradorId = dto.ColaboradorId,
                CriadoPorId = criadoPorId,
                Justificativa = dto.Justificativa,
                DepartamentoId = dto.DepartamentoId,
                MoedaId = dto.MoedaId,
                Valor = dto.Valor,
                DataPagamentoRequerida = dto.DataPagamentoRequerida,
                DataPagamentoAjustada = dataAjustada,
                Observacoes = dto.Observacoes,
                Status = StatusAdiantamento.Pendente,
                CriadoEm = DateTime.UtcNow
            };

            _context.SolicitacoesAdiantamento.Add(solicitacao);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Solicitação ID {Id} salva no banco.", solicitacao.Id);

            await LogAuditoriaAsync(criadoPorId, "SolicitacaoAdiantamento", solicitacao.Id, "CREATE");
            return solicitacao;
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

        // ---------------------------------------------------------------------
        // R - READ (Listagem)
        // ---------------------------------------------------------------------
        public async Task<IEnumerable<SolicitacaoAdiantamentoListDto>> GetAllAsync(string? search, string? status, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Moeda)
                .AsQueryable();

            // Lógica de filtragem
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.Justificativa.Contains(search)
                                         || s.Colaborador!.NomeCompleto.Contains(search));
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusAdiantamento>(status, true, out var statusEnum))
            {
                query = query.Where(s => s.Status == statusEnum);
            }

            if (dataInicial.HasValue)
            {
                query = query.Where(s => s.CriadoEm >= dataInicial.Value.Date);
            }
            if (dataFinal.HasValue)
            {
                query = query.Where(s => s.CriadoEm < dataFinal.Value.Date.AddDays(1));
            }

            var results = await query
                .OrderByDescending(s => s.CriadoEm)
                .Select(s => new SolicitacaoAdiantamentoListDto
                {
                    Id = s.Id,
                    SolicitanteNome = s.Colaborador!.NomeCompleto,
                    Descricao = s.Justificativa,
                    Valor = s.Valor,
                    MoedaCodigo = s.Moeda!.Codigo,
                    ValorFormatado = $"{s.Moeda.Simbolo} {s.Valor:N2}",
                    DataCriacao = s.CriadoEm,
                    Status = s.Status,
                    StatusDescricao = s.Status.ToString(),
                })
                .ToListAsync();

            return results;
        }

        // ---------------------------------------------------------------------
        // R - READ (Detalhes - Usando o DTO completo para visualização)
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamentoDetailDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Service: Buscando detalhes do Adiantamento ID {Id}.", id);

            var result = await _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Departamento)
                .Include(s => s.Moeda)
                .Include(s => s.CriadoPor) // Para obter o CriadoPorNome
                .Where(s => s.Id == id)
                .Select(s => new SolicitacaoAdiantamentoDetailDto
                {
                    // Campos da Listagem (Base)
                    Id = s.Id,
                    SolicitanteNome = s.Colaborador!.NomeCompleto,
                    Descricao = s.Justificativa,
                    Valor = s.Valor,
                    MoedaCodigo = s.Moeda!.Codigo,
                    ValorFormatado = $"{s.Moeda.Simbolo} {s.Valor:N2}",
                    DataCriacao = s.CriadoEm,
                    Status = s.Status,
                    StatusDescricao = s.Status.ToString(),

                    // Campos específicos do Detalhe
                    JustificativaCompleta = s.Justificativa,
                    DepartamentoNome = s.Departamento!.Nome,
                    DataPagamentoRequerida = s.DataPagamentoRequerida,
                    DataPagamentoAjustada = s.DataPagamentoAjustada,
                    Observacoes = s.Observacoes,
                    CriadoPorNome = s.CriadoPor!.NomeCompleto,
                    Anexos = new List<string> { "Recibo.pdf", "Comprovante.jpg" } // Mock
                })
                .FirstOrDefaultAsync();

            if (result != null)
            {
                _logger.LogDebug("Service: Detalhes do Adiantamento ID {Id} encontrados no DB.", id);
            }

            return result;
        }

        // ---------------------------------------------------------------------
        // U - UPDATE
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamento> UpdateAsync(int id, SolicitacaoAdiantamentoCreateDto dto)
        {
            var solicitacao = await _context.SolicitacoesAdiantamento.FindAsync(id);

            if (solicitacao == null)
            {
                throw new KeyNotFoundException($"Solicitação de Adiantamento com ID {id} não encontrada.");
            }

            if (solicitacao.Status != StatusAdiantamento.Pendente && solicitacao.Status != StatusAdiantamento.EmRevisao)
            {
                throw new InvalidOperationException("Apenas solicitações Pendentes ou em Revisão podem ser editadas.");
            }

            var dataAjustada = await _holidayService.GetNextBusinessDayAsync(dto.DataPagamentoRequerida);

            solicitacao.ColaboradorId = dto.ColaboradorId;
            solicitacao.Justificativa = dto.Justificativa;
            solicitacao.DepartamentoId = dto.DepartamentoId;
            solicitacao.MoedaId = dto.MoedaId;
            solicitacao.Valor = dto.Valor;
            solicitacao.DataPagamentoRequerida = dto.DataPagamentoRequerida;
            solicitacao.DataPagamentoAjustada = dataAjustada;
            solicitacao.Observacoes = dto.Observacoes;
            solicitacao.AtualizadoEm = DateTime.UtcNow;

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();

            await LogAuditoriaAsync(1, "SolicitacaoAdiantamento", solicitacao.Id, "UPDATE");
            return solicitacao;
        }

        // ---------------------------------------------------------------------
        // AÇÃO DE NEGÓCIO: Mudar Status
        // ---------------------------------------------------------------------
        public async Task ChangeStatusAsync(int id, StatusAdiantamento novoStatus)
        {
            var solicitacao = await _context.SolicitacoesAdiantamento.FindAsync(id);

            if (solicitacao == null)
            {
                throw new KeyNotFoundException($"Solicitação de Adiantamento com ID {id} não encontrada.");
            }

            solicitacao.Status = novoStatus;
            solicitacao.AtualizadoEm = DateTime.UtcNow;

            await LogAuditoriaAsync(1, "SolicitacaoAdiantamento", solicitacao.Id, $"STATUS_CHANGE_TO_{novoStatus.ToString().ToUpper()}");

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();
        }
    }
}