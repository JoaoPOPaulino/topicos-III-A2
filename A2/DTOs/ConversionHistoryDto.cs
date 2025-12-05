namespace A2.DTOs
{
    public class ConversionHistoryDto
    {
        public int Id { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Converted { get; set; }
        public decimal Rate { get; set; }
        public string Date { get; set; } = string.Empty;
    }
}
