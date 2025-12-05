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
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ExchangeRateService> _logger;

        // ✅ API correta: https://economia.awesomeapi.com.br/last/USD-BRL
        private const string AwesomeApiLastUrl = "https://economia.awesomeapi.com.br/last/";

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

            _logger.LogInformation("🔄 Buscando taxa: {Base} → {Cotada}", moedaBase.Codigo, moedaCotada.Codigo);

            string cacheKey = $"rate_{moedaBaseId}_{moedaCotadaId}_{date:yyyy-MM-dd}";
            decimal rate = 0;

            // 1. Verificar cache em memória
            if (_memoryCache.TryGetValue(cacheKey, out rate))
            {
                _logger.LogInformation("✅ Taxa encontrada no CACHE: {Base}/{Cotada} = {Rate}",
                    moedaBase.Codigo, moedaCotada.Codigo, rate);
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
                _logger.LogInformation("✅ Taxa encontrada no BANCO: {Base}/{Cotada} = {Rate}",
                    moedaBase.Codigo, moedaCotada.Codigo, rate);
                return rate;
            }

            // 3. Buscar na API externa (protegida pelo Polly)
            try
            {
                _logger.LogInformation("🌐 Consultando AwesomeAPI...");
                rate = await GetRateFromAwesomeApi(moedaBase.Codigo, moedaCotada.Codigo, date);

                // 4. Salvar no banco e cache
                if (rate > 0)
                {
                    _logger.LogInformation("✅ Taxa obtida da API: {Base}/{Cotada} = {Rate}",
                        moedaBase.Codigo, moedaCotada.Codigo, rate);

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
                _logger.LogWarning(ex, "⚠️ Falha na AwesomeAPI. Tentando fallback...");

                // Fallback 1: Taxa mais recente do banco
                rate = await GetMostRecentRate(moedaBaseId, moedaCotadaId);

                if (rate > 0)
                {
                    _logger.LogWarning("✅ Usando taxa recente do banco: {Rate}", rate);
                    _memoryCache.Set(cacheKey, rate, TimeSpan.FromHours(24));
                    return rate;
                }

                // Fallback 2: Taxa de emergência (hardcoded)
                rate = GetEmergencyRate(moedaBase.Codigo, moedaCotada.Codigo);
                _logger.LogError("⚠️ Usando taxa de EMERGÊNCIA: {Rate}", rate);

                return rate;
            }

            return rate;
        }

        private async Task<decimal> GetRateFromAwesomeApi(string moedaBase, string moedaCotada, DateTime date)
        {
            // ✅ AwesomeAPI sempre retorna a taxa de X para BRL
            // Exemplo: USD-BRL retorna quanto vale 1 USD em BRL

            if (moedaBase != "BRL" && moedaCotada != "BRL")
            {
                throw new InvalidOperationException("A AwesomeAPI só suporta conversão que envolva BRL.");
            }

            var client = _httpClientFactory.CreateClient("AwesomeApiCambiaria");

            // ✅ CORREÇÃO: Determinar o par correto
            string pair;
            bool needsInversion = false;

            if (moedaBase == "BRL")
            {
                // BRL → USD: API retorna USD-BRL (quanto vale 1 USD em BRL)
                // Precisamos inverter: 1 / (USD-BRL) = BRL-USD
                pair = $"{moedaCotada}-{moedaBase}"; // USD-BRL
                needsInversion = true;
            }
            else
            {
                // USD → BRL: API retorna USD-BRL (quanto vale 1 USD em BRL)
                // Já é o que queremos
                pair = $"{moedaBase}-{moedaCotada}"; // USD-BRL
                needsInversion = false;
            }

            // ✅ Usar endpoint /last para pegar cotação mais recente
            string url = $"{AwesomeApiLastUrl}{pair}";

            _logger.LogInformation("📡 Chamando API: {Url}", url);

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ API retornou status {Status}", response.StatusCode);
                throw new HttpRequestException($"AwesomeAPI retornou {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("📄 Resposta da API: {Json}", json);

            // ✅ Parse da resposta
            // Formato: { "USDBRL": { "bid": "5.3100", ... } }
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Remove hífen do par para acessar a propriedade
            var pairKey = pair.Replace("-", "");

            if (!root.TryGetProperty(pairKey, out var rateData))
            {
                _logger.LogWarning("❌ Par {Pair} não encontrado na resposta", pairKey);
                throw new InvalidOperationException($"Par {pair} não encontrado na resposta da API");
            }

            if (!rateData.TryGetProperty("bid", out var bidElement))
            {
                _logger.LogWarning("❌ Campo 'bid' não encontrado");
                throw new InvalidOperationException("Campo 'bid' não encontrado na resposta");
            }

            var bidString = bidElement.GetString();

            if (!decimal.TryParse(bidString, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal apiRate))
            {
                _logger.LogWarning("❌ Erro ao converter taxa: {Bid}", bidString);
                throw new InvalidOperationException($"Erro ao converter taxa: {bidString}");
            }

            // ✅ Inverter se necessário
            decimal finalRate = needsInversion ? (1.0m / apiRate) : apiRate;

            _logger.LogInformation("✅ Taxa da API: {ApiRate}, Final: {FinalRate} (Invertido: {Inverted})",
                apiRate, finalRate, needsInversion);

            return finalRate;
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
            // ✅ Taxas de emergência CORRETAS (1 USD = 5.31 BRL, então 1 BRL = 0.188 USD)
            var emergencyRates = new Dictionary<string, decimal>
            {
                // Quanto vale 1 moeda da chave em reais
                ["USD-BRL"] = 5.31m,  // 1 USD = 5.31 BRL
                ["EUR-BRL"] = 5.80m,  // 1 EUR = 5.80 BRL
                ["GBP-BRL"] = 6.80m,  // 1 GBP = 6.80 BRL
            };

            // Tenta BRL → Moeda
            if (moedaBase == "BRL")
            {
                string key = $"{moedaCotada}-{moedaBase}";
                if (emergencyRates.TryGetValue(key, out var rate))
                {
                    return 1.0m / rate; // Inverte
                }
            }

            // Tenta Moeda → BRL
            if (moedaCotada == "BRL")
            {
                string key = $"{moedaBase}-{moedaCotada}";
                if (emergencyRates.TryGetValue(key, out var rate))
                {
                    return rate;
                }
            }

            _logger.LogWarning("⚠️ Nenhuma taxa de emergência encontrada, retornando 1.0");
            return 1.0m;
        }
    }
}