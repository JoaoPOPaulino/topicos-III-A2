using A2.DTOs;
using A2.Models;
using System.Threading.Tasks;

namespace A2.Services
{
    public interface IPrestacaoContasService
    {
        /// <summary>
        /// Cria a Prestação de Contas, calcula a conversão das despesas e o saldo final.
        /// </summary>
        /// <param name="dto">Dados do relatório e despesas.</param>
        /// <param name="criadoPorId">ID do usuário (RH) que está criando o relatório.</param>
        /// <returns>O objeto PrestacaoContas criado.</returns>
        Task<PrestacaoContas> CreateAsync(PrestacaoContasCreateDto dto, int criadoPorId);
    }
}