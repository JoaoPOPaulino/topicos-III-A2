using A2.Data;
using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models;
using A2.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // Necessário para exceções como NotFound

namespace A2.Services
{
    // A classe deve herdar de ISolicitacaoAdiantamentoService
    public class SolicitacaoAdiantamentoService : ISolicitacaoAdiantamentoService
    {
        private readonly ApplicationDbContext _context;

        public SolicitacaoAdiantamentoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // C - CREATE
        // ---------------------------------------------------------------------
        public async Task<SolicitacaoAdiantamento> CreateAsync(SolicitacaoAdiantamentoCreateDto dto, int criadoPorId)
        {
            var solicitacao = new SolicitacaoAdiantamento
            {
                ColaboradorId = dto.ColaboradorId,
                CriadoPorId = criadoPorId,
                Justificativa = dto.Justificativa,
                DepartamentoId = dto.DepartamentoId,
                MoedaId = dto.MoedaId,
                Valor = dto.Valor,
                DataPagamentoRequerida = dto.DataPagamentoRequerida,
                Status = StatusAdiantamento.Pendente,
                CriadoEm = DateTime.UtcNow
            };

            _context.SolicitacoesAdiantamento.Add(solicitacao);
            await _context.SaveChangesAsync();
            return solicitacao;
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

            // Atualiza os campos:
            solicitacao.ColaboradorId = dto.ColaboradorId;
            solicitacao.Justificativa = dto.Justificativa;
            solicitacao.DepartamentoId = dto.DepartamentoId;
            solicitacao.MoedaId = dto.MoedaId;
            solicitacao.Valor = dto.Valor;
            solicitacao.DataPagamentoRequerida = dto.DataPagamentoRequerida;
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

            // Lógica de transição de status (Ex: você não pode ir de Pago para Pendente)
            // (Para simplicidade inicial, apenas atualizamos o status)
            solicitacao.Status = novoStatus;
            solicitacao.AtualizadoEm = DateTime.UtcNow;

            // Log de Auditoria seria adicionado aqui! (RF05)

            _context.SolicitacoesAdiantamento.Update(solicitacao);
            await _context.SaveChangesAsync();
        }
    }
}