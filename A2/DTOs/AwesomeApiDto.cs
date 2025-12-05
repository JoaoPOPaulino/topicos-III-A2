namespace A2.DTOs
{
    public class AwesomeApiRateResponse
    {
        public string Code { get; set; } = string.Empty;
        public string Codein { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string High { get; set; } = string.Empty;
        public string Low { get; set; } = string.Empty;
        public string VarBid { get; set; } = string.Empty;
        public string PctChange { get; set; } = string.Empty;
        public string Bid { get; set; } = string.Empty; // ✅ Taxa de compra (usada na conversão)
        public string Ask { get; set; } = string.Empty; // Taxa de venda
        public string Timestamp { get; set; } = string.Empty;
        public string Create_date { get; set; } = string.Empty;
    }
}
