using A2.DTOs;
using A2.DTOs.SolicitacaoAdiantamento;
using A2.Models;
using A2.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A2.Services
{
    public interface ISolicitacaoAdiantamentoService
    {
        Task<SolicitacaoAdiantamento> CreateAsync(SolicitacaoAdiantamentoCreateDto dto, int criadoPorId);

        Task<SolicitacaoAdiantamento> UpdateAsync(int id, SolicitacaoAdiantamentoCreateDto dto);

        Task<IEnumerable<SolicitacaoAdiantamentoListDto>> GetAllAsync(string? search, string? status, DateTime? dataInicial, DateTime? dataFinal);

        Task<SolicitacaoAdiantamentoDetailDto?> GetByIdAsync(int id);

        Task ChangeStatusAsync(int id, StatusAdiantamento novoStatus);

        Task<bool> IsHolidayAsync(DateTime date);
        Task<DateTime> GetNextBusinessDayAsync(DateTime date);

    }
}