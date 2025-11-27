namespace A2.Services
{
    public interface IHolidayService
    {
        // Retorna a lista de feriados para o ano, em cache.
        Task<IEnumerable<DateTime>> GetNationalHolidaysAsync(int year);

        // Verifica se uma data é feriado.
        Task<bool> IsHolidayAsync(DateTime date);

        // Retorna o próximo dia útil.
        Task<DateTime> GetNextBusinessDayAsync(DateTime date);
    }
}