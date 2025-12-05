using A2.DTOs;
using A2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace A2.Service
{
    public interface ICurrencyService
    {
        Task<ConversionResultDto> GetConversionAsync(string from, string to, decimal amount);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly Data.ApplicationDbContext _context;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(
            IExchangeRateService exchangeRateService,
            Data.ApplicationDbContext context,
            ILogger<CurrencyService> logger)
        {
            _exchangeRateService = exchangeRateService;
            _context = context;
            _logger = logger;
        }

        public async Task<ConversionResultDto> GetConversionAsync(string from, string to, decimal amount)
        {
            try
            {
                _logger.LogInformation("Iniciando conversão: {Amount} {From} → {To}", amount, from, to);

                // ✅ Validação: Códigos de moeda devem ter 3 caracteres
                if (string.IsNullOrWhiteSpace(from) || from.Length != 3)
                    throw new ArgumentException("Código da moeda origem inválido.", nameof(from));

                if (string.IsNullOrWhiteSpace(to) || to.Length != 3)
                    throw new ArgumentException("Código da moeda destino inválido.", nameof(to));

                if (amount <= 0)
                    throw new ArgumentException("O valor deve ser maior que zero.", nameof(amount));

                // Normaliza para maiúsculas
                from = from.ToUpper();
                to = to.ToUpper();

                // ✅ Busca moedas no banco de dados
                var moedaFrom = await _context.Moedas
                    .FirstOrDefaultAsync(m => m.Codigo == from);

                var moedaTo = await _context.Moedas
                    .FirstOrDefaultAsync(m => m.Codigo == to);

                if (moedaFrom == null)
                {
                    _logger.LogWarning("Moeda origem não encontrada: {From}", from);
                    throw new InvalidOperationException($"Moeda '{from}' não encontrada no sistema.");
                }

                if (moedaTo == null)
                {
                    _logger.LogWarning("Moeda destino não encontrada: {To}", to);
                    throw new InvalidOperationException($"Moeda '{to}' não encontrada no sistema.");
                }

                // ✅ Conversão direta se for a mesma moeda
                if (from == to)
                {
                    _logger.LogInformation("Mesma moeda, retornando valor original");
                    return new ConversionResultDto
                    {
                        From = from,
                        To = to,
                        Amount = amount,
                        Converted = amount,
                        Rate = 1.0m,
                        Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                }

                // ✅ Obtém taxa de câmbio usando o ExchangeRateService existente
                var hoje = DateTime.Today;
                decimal rate;

                try
                {
                    rate = await _exchangeRateService.GetRateAsync(moedaFrom.Id, moedaTo.Id, hoje);
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("AwesomeAPI só suporta"))
                {
                    // ✅ FALLBACK: Se AwesomeAPI não suporta o par (ex: USD→EUR)
                    // Tenta conversão triangular via BRL
                    _logger.LogWarning("Par {From}/{To} não suportado pela AwesomeAPI. Tentando conversão triangular via BRL.", from, to);

                    rate = await GetTriangularRateAsync(moedaFrom.Id, moedaTo.Id, hoje);
                }

                var converted = amount * rate;

                var result = new ConversionResultDto
                {
                    From = from,
                    To = to,
                    Amount = amount,
                    Converted = Math.Round(converted, 2),
                    Rate = Math.Round(rate, 6),
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _logger.LogInformation("✅ Conversão concluída: {Amount} {From} = {Converted} {To} (Taxa: {Rate})",
                    amount, from, converted, to, rate);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar conversão de {From} para {To}", from, to);
                throw;
            }
        }

        /// <summary>
        /// Conversão triangular: FROM → BRL → TO
        /// Útil quando a API não suporta conversão direta entre moedas não-BRL
        /// </summary>
        private async Task<decimal> GetTriangularRateAsync(int moedaFromId, int moedaToId, DateTime date)
        {
            _logger.LogInformation("Executando conversão triangular via BRL");

            // Busca moeda BRL
            var moedaBrl = await _context.Moedas
                .FirstOrDefaultAsync(m => m.Codigo == "BRL");

            if (moedaBrl == null)
            {
                _logger.LogError("Moeda BRL não encontrada no sistema");
                throw new InvalidOperationException("Moeda BRL não encontrada.");
            }

            // FROM → BRL
            var rateFromToBrl = await _exchangeRateService.GetRateAsync(moedaFromId, moedaBrl.Id, date);

            // BRL → TO
            var rateBrlToTo = await _exchangeRateService.GetRateAsync(moedaBrl.Id, moedaToId, date);

            // Taxa final: FROM → BRL → TO
            var triangularRate = rateFromToBrl * rateBrlToTo;

            _logger.LogInformation("Taxa triangular calculada: {Rate}", triangularRate);

            return triangularRate;
        }
    }
}