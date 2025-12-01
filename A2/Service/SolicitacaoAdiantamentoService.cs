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
    // A classe deve herdar de ISolicitacaoAdiantamentoService
    public class SolicitacaoAdiantamentoService : ISolicitacaoAdiantamentoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHolidayService _holidayService;
        private readonly ILogger<SolicitacaoAdiantamentoService> _logger; // Assume que ILogger está injetado

        public SolicitacaoAdiantamentoService(
            ApplicationDbContext context,
            IHolidayService holidayService,
            ILogger<SolicitacaoAdiantamentoService> logger) // Adicionado ILogger
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
            // Validações...
            var colaboradorExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.ColaboradorId && u.Ativo);

            if (!colaboradorExiste)
                throw new InvalidOperationException("Colaborador não encontrado ou inativo.");

            // ... (Validação de Adiantamento pendente e Data no passado)

            // AÇÃO DE NEGÓCIO: Ajustar Data de Pagamento para o próximo dia útil
            var dataAjustada = await _holidayService.GetNextBusinessDayAsync(dto.DataPagamentoRequerida);

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

            await LogAuditoriaAsync(criadoPorId, "SolicitacaoAdiantamento", solicitacao.Id, "CREATE");
            return solicitacao;
        }

        private async Task LogAuditoriaAsync(int usuarioId, string entidade, int entidadeId, string acao)
        {
            // Implementação LogAuditoria (simplificada)
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
            // Implementação de GetAllAsync (mantida)
            var query = _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Moeda)
                .AsQueryable();

            // ... (Lógica de filtragem) ...

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
                    StatusDescricao = s.Status.ToString(), // Usando ToString() para o nome do enum
                })
                .ToListAsync();

            return results;
        }

        // ---------------------------------------------------------------------
        // R - READ (Detalhes - Usando o novo DTO)
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamentoDetailDto?> GetByIdAsync(int id)
        {
            var result = await _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Departamento)
                .Include(s => s.Moeda)
                // Usar ThenInclude para incluir a navegação de CriadoPor
                .Include(s => s.CriadoPor)
                .Where(s => s.Id == id)
                .Select(s => new SolicitacaoAdiantamentoDetailDto // Usar o DTO de detalhe
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

                    // Campos específicos do Detail DTO
                    JustificativaCompleta = s.Justificativa,
                    DepartamentoNome = s.Departamento!.Nome,
                    DataPagamentoRequerida = s.DataPagamentoRequerida,
                    DataPagamentoAjustada = s.DataPagamentoAjustada,
                    Observacoes = s.Observacoes,
                    CriadoPorNome = s.CriadoPor!.NomeCompleto,
                    Anexos = new List<string> { "Recibo.pdf", "Comprovante.jpg" } // Mock
                })
                .FirstOrDefaultAsync();

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

            // AÇÃO DE NEGÓCIO: Reajustar Data de Pagamento para o próximo dia útil
            var dataAjustada = await _holidayService.GetNextBusinessDayAsync(dto.DataPagamentoRequerida);

            // Atualiza os campos:
            solicitacao.ColaboradorId = dto.ColaboradorId;
            solicitacao.Justificativa = dto.Justificativa;
            solicitacao.DepartamentoId = dto.DepartamentoId;
            solicitacao.MoedaId = dto.MoedaId;
            solicitacao.Valor = dto.Valor;
            solicitacao.DataPagamentoRequerida = dto.DataPagamentoRequerida;
            solicitacao.DataPagamentoAjustada = dataAjustada; // Atualizado
            solicitacao.Observacoes = dto.Observacoes; // Adicionado
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

            int usuarioOperadorId = 1;
            await LogAuditoriaAsync(usuarioOperadorId, "SolicitacaoAdiantamento", solicitacao.Id, $"STATUS_CHANGE_TO_{novoStatus.ToString().ToUpper()}");

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();
        }
    }
}