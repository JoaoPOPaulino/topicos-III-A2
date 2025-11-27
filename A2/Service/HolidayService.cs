using System.Text.Json;
using A2.Data;
using Microsoft.EntityFrameworkCore;
using A2.Models;

namespace A2.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private const string BrasilApiUrl = "https://brasilapi.com.br/api/feriados/v1/";

        public HolidayService(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DateTime>> GetNationalHolidaysAsync(int year)
        {
            // Tenta obter do cache (tabela Feriado)
            var cachedHolidays = await _context.Feriados
                .Where(f => f.Data.Year == year)
                .Select(f => f.Data.Date)
                .ToListAsync();

            if (cachedHolidays.Any())
                return cachedHolidays;

            // Se não estiver no cache, consulta a API
            try
            {
                var response = await _httpClient.GetAsync(BrasilApiUrl + year);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                // Assumindo a estrutura de resposta da BrasilAPI: [{date: "YYYY-MM-DD", name: "Nome", type: "national"}]
                using (var doc = JsonDocument.Parse(json))
                {
                    var newHolidays = new List<Feriado>();
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        var dateStr = element.GetProperty("date").GetString();
                        if (DateTime.TryParse(dateStr, out var date))
                        {
                            var feriado = new Feriado
                            {
                                Nome = element.GetProperty("name").GetString() ?? "Feriado Nacional",
                                Data = date.Date,
                                Uf = "BR" // Marcando como Nacional
                            };
                            newHolidays.Add(feriado);
                            cachedHolidays.Add(date.Date);
                        }
                    }

                    // Salva no cache
                    _context.Feriados.AddRange(newHolidays);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Em caso de falha da API, retorna o que tem ou vazio
                return cachedHolidays;
            }

            return cachedHolidays;
        }

        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            var holidays = await GetNationalHolidaysAsync(date.Year);

            // Fim de semana OU Feriado
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday ||
                   holidays.Contains(date.Date);
        }

        public async Task<DateTime> GetNextBusinessDayAsync(DateTime date)
        {
            var nextDay = date.Date;
            // Loop até encontrar um dia que não seja feriado nem fim de semana
            while (await IsHolidayAsync(nextDay))
            {
                nextDay = nextDay.AddDays(1);
            }
            return nextDay;
        }
    }
}