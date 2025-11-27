// Services/IExchangeRateService.cs

namespace A2.Services
{
    public interface IExchangeRateService
    {
        /// <summary>
        /// Obtém a taxa de câmbio de uma Moeda Base para uma Moeda Cotada em uma data específica.
        /// Se não estiver em cache, busca na API externa e armazena.
        /// </summary>
        /// <param name="moedaBaseId">ID da moeda base (ex: BRL).</param>
        /// <param name="moedaCotadaId">ID da moeda cotada (ex: USD).</param>
        /// <param name="date">Data de referência da cotação.</param>
        /// <returns>A taxa de câmbio (ex: 5.23).</returns>
        Task<decimal> GetRateAsync(int moedaBaseId, int moedaCotadaId, DateTime date);
    }
}