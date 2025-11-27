using A2.Data;
using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models;
using A2.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace A2.Services
{
    // A classe deve herdar de ISolicitacaoAdiantamentoService
    public class SolicitacaoAdiantamentoService : ISolicitacaoAdiantamentoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHolidayService _holidayService; // INJEÇÃO: Serviço de Feriados

        public SolicitacaoAdiantamentoService(
            ApplicationDbContext context,
            IHolidayService holidayService) // Construtor atualizado
        {
            _context = context;
            _holidayService = holidayService;
        }

        // ---------------------------------------------------------------------
        // C - CREATE
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamento> CreateAsync(SolicitacaoAdiantamentoCreateDto dto, int criadoPorId)
        {
            // Validar se o colaborador existe e está ativo
            var colaboradorExiste = await _context.Usuarios
                .AnyAsync(u => u.Id == dto.ColaboradorId && u.Ativo);

            if (!colaboradorExiste)
                throw new InvalidOperationException("Colaborador não encontrado ou inativo.");

            // Validar se há adiantamentos pendentes para o colaborador
            var temAdiantamentoPendente = await _context.SolicitacoesAdiantamento
                .AnyAsync(s => s.ColaboradorId == dto.ColaboradorId
                             && (s.Status == StatusAdiantamento.Pendente
                             || s.Status == StatusAdiantamento.Aprovado
                             || s.Status == StatusAdiantamento.PrestacaoPendente));

            if (temAdiantamentoPendente)
                throw new InvalidOperationException("Colaborador possui adiantamento pendente (Pendente, Aprovado ou Prestação Pendente).");

            // Validar data de pagamento (não pode ser no passado)
            if (dto.DataPagamentoRequerida.Date < DateTime.Today)
                throw new InvalidOperationException("Data de pagamento não pode ser no passado.");

            // AÇÃO DE NEGÓCIO: Ajustar Data de Pagamento para o próximo dia útil (RF04.6)
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
                DataPagamentoAjustada = dataAjustada, // Adicionado campo Data Ajustada
                Observacoes = dto.Observacoes,
                Status = StatusAdiantamento.Pendente,
                CriadoEm = DateTime.UtcNow
            };

            _context.SolicitacoesAdiantamento.Add(solicitacao);
            await _context.SaveChangesAsync();

            // Log de auditoria
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
        // R - READ (Listagem para a tela principal)
        // ---------------------------------------------------------------------
        public async Task<IEnumerable<SolicitacaoAdiantamentoListDto>> GetAllAsync(string? search, string? status, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Moeda)
                .AsQueryable();

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
                // Inclui o dia inteiro
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
                    StatusDescricao = s.Status.ToString().ToLower()
                })
                .ToListAsync();

            return results;
        }

        // ---------------------------------------------------------------------
        // R - READ (Detalhes para a tela 'Ver Adiantamento')
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamentoListDto?> GetByIdAsync(int id)
        {
            var result = await _context.SolicitacoesAdiantamento
                .Include(s => s.Colaborador)
                .Include(s => s.Departamento)
                .Include(s => s.Moeda)
                .Where(s => s.Id == id)
                .Select(s => new SolicitacaoAdiantamentoListDto // Reutilizando o DTO de listagem para detalhes
                {
                    Id = s.Id,
                    SolicitanteNome = s.Colaborador!.NomeCompleto,
                    Descricao = s.Justificativa,
                    Valor = s.Valor,
                    MoedaCodigo = s.Moeda!.Codigo,
                    ValorFormatado = $"{s.Moeda.Simbolo} {s.Valor:N2}",
                    DataCriacao = s.CriadoEm,
                    Status = s.Status,
                    StatusDescricao = s.Status.ToString().ToLower()
                    // Adicione mais campos aqui se o DTO de listagem não for suficiente para a tela 'Ver Detalhes'
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
            solicitacao.AtualizadoEm = DateTime.UtcNow;

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();
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

            // Lógica de transição de status (Simplificada)
            // Futuramente: Adicionar validações de transição de status.
            solicitacao.Status = novoStatus;
            solicitacao.AtualizadoEm = DateTime.UtcNow;

            // Log de Auditoria
            // Assumindo que a operação é feita pelo usuário Admin FinOps (RH), ID 1.
            int usuarioOperadorId = 1;
            await LogAuditoriaAsync(usuarioOperadorId, "SolicitacaoAdiantamento", solicitacao.Id, $"STATUS_CHANGE_TO_{novoStatus.ToString().ToUpper()}");

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();
        }
    }
}