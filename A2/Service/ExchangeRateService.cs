// Services/ExchangeRateService.cs

using A2.Data;
using A2.DTOs;
using A2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace A2.Services
{
    // Interface IExchangeRateService é assumida
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ExchangeRateService> _logger;

        private const string AwesomeApiBaseUrl = "https://economia.awesomeapi.com.br/json/daily/";

        public ExchangeRateService(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            ILogger<ExchangeRateService> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<decimal> GetRateAsync(int moedaBaseId, int moedaCotadaId, DateTime date)
        {
            if (moedaBaseId == moedaCotadaId)
                return 1.0m;

            var moedaBase = await _context.Moedas.FindAsync(moedaBaseId)
                ?? throw new KeyNotFoundException($"Moeda base ID {moedaBaseId} não encontrada.");
            var moedaCotada = await _context.Moedas.FindAsync(moedaCotadaId)
                ?? throw new KeyNotFoundException($"Moeda cotada ID {moedaCotadaId} não encontrada.");

            string cacheKey = $"rate_{moedaBaseId}_{moedaCotadaId}_{date:yyyy-MM-dd}";
            decimal rate = 0;

            // 1. Verificar cache em memória
            if (_memoryCache.TryGetValue(cacheKey, out rate))
            {
                _logger.LogInformation($"Taxa de {moedaCotada.Codigo}-{moedaBase.Codigo} ({date:yyyy-MM-dd}) encontrada no cache de memória.");
                return rate;
            }

            // 2. Verificar cache no banco de dados
            rate = await _context.LogsCotacao
                .Where(l => l.MoedaBaseId == moedaBaseId &&
                           l.MoedaCotadaId == moedaCotadaId &&
                           l.DataReferencia.Date == date.Date)
                .Select(l => l.Taxa)
                .FirstOrDefaultAsync();

            if (rate > 0)
            {
                _memoryCache.Set(cacheKey, rate, TimeSpan.FromHours(24));
                _logger.LogInformation($"Taxa de {moedaCotada.Codigo}-{moedaBase.Codigo} ({date:yyyy-MM-dd}) encontrada no banco.");
                return rate;
            }

            // 3. Buscar na API externa (protegida pelo Polly)
            try
            {
                // Se a API falhar (Polly desiste), uma exceção será lançada aqui
                rate = await GetRateFromAwesomeApi(moedaBase.Codigo, moedaCotada.Codigo, date);

                // 4. Salvar no banco e cache (APENAS se veio com sucesso da API)
                if (rate > 0)
                {
                    var log = new LogCotacao
                    {
                        MoedaBaseId = moedaBaseId,
                        MoedaCotadaId = moedaCotadaId,
                        Taxa = rate,
                        DataReferencia = date.Date,
                        CapturadoEm = DateTime.UtcNow
                    };
                    _context.LogsCotacao.Add(log);
                    await _context.SaveChangesAsync();
                    _memoryCache.Set(cacheKey, rate, TimeSpan.FromHours(24));
                    return rate;
                }
            }
            catch (Exception ex)
            {
                // O Polly falhou (Taxa de requisição excedida, falha de rede persistente, etc.)
                _logger.LogWarning(ex, $"Falha persistente na AwesomeAPI. Tentando fallback para taxa recente...");

                // Tentativa de Fallback 1: Taxa mais recente do banco
                rate = await GetMostRecentRate(moedaBaseId, moedaCotadaId);

                if (rate > 0)
                {
                    _logger.LogWarning($"Usando taxa recente ({rate}) do banco como fallback.");
                    _memoryCache.Set(cacheKey, rate, TimeSpan.FromHours(24));
                    return rate;
                }

                // Tentativa de Fallback 2: Taxa de emergência (hardcoded)
                rate = GetEmergencyRate(moedaBase.Codigo, moedaCotada.Codigo);
                _logger.LogError($"Falha total na cotação. Usando taxa de emergência: {rate}");

                return rate;
            }

            return rate;
        }

        private async Task<decimal> GetRateFromAwesomeApi(string moedaBase, string moedaCotada, DateTime date)
        {
            if (moedaBase != "BRL" && moedaCotada != "BRL")
            {
                throw new InvalidOperationException("A AwesomeAPI só suporta conversão que envolva BRL.");
            }
            string pair = $"{moedaCotada}-{moedaBase}";

            var client = _httpClientFactory.CreateClient("AwesomeApiCambiaria");
            long timestamp = new DateTimeOffset(date.Date).ToUnixTimeSeconds();
            string url = $"{AwesomeApiBaseUrl}{pair}/1?end_date={timestamp}";

            // O cliente com Polly será usado aqui
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Lança exceção se todas as retentativas falharem

            var json = await response.Content.ReadAsStringAsync();

            // Assumindo que AwesomeApiRateResponse é a classe correta
            var rates = JsonSerializer.Deserialize<List<AwesomeApiRateResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (rates == null || !rates.Any())
                throw new InvalidOperationException($"Nenhuma cotação encontrada para {pair} na data {date.ToShortDateString()}.");

            if (!decimal.TryParse(rates.First().Bid, out decimal rate))
                throw new InvalidOperationException("Erro ao converter taxa.");

            return rate;
        }

        private async Task<decimal> GetMostRecentRate(int moedaBaseId, int moedaCotadaId)
        {
            var recentRate = await _context.LogsCotacao
                .Where(l => l.MoedaBaseId == moedaBaseId && l.MoedaCotadaId == moedaCotadaId)
                .OrderByDescending(l => l.DataReferencia)
                .Select(l => l.Taxa)
                .FirstOrDefaultAsync();

            return recentRate;
        }

        private decimal GetEmergencyRate(string moedaBase, string moedaCotada)
        {
            var emergencyRates = new Dictionary<string, decimal>
            {
                ["USD-BRL"] = 5.00m,
                ["EUR-BRL"] = 5.50m,
            };

            string key = $"{moedaCotada}-{moedaBase}";

            if (emergencyRates.TryGetValue(key, out var rate))
                return rate;

            // Lógica para conversão inversa (ex: BRL-USD)
            string inverseKey = $"{moedaBase}-{moedaCotada}";

            if (emergencyRates.TryGetValue(inverseKey, out rate))
            {
                return 1.0m / rate;
            }

            return 1.0m;
        }
    }
}