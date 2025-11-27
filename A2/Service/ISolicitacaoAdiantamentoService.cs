using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models;
using A2.Models.Enums;

namespace A2.Services
{
    public interface ISolicitacaoAdiantamentoService
    {
        Task<SolicitacaoAdiantamento> CreateAsync(SolicitacaoAdiantamentoCreateDto dto, int criadoPorId);
        Task<SolicitacaoAdiantamento> UpdateAsync(int id, SolicitacaoAdiantamentoCreateDto dto);
        Task<SolicitacaoAdiantamentoListDto?> GetByIdAsync(int id);
        Task<IEnumerable<SolicitacaoAdiantamentoListDto>> GetAllAsync(string? search, string? status, DateTime? dataInicial, DateTime? dataFinal);

        Task ChangeStatusAsync(int id, StatusAdiantamento novoStatus);
    }
}