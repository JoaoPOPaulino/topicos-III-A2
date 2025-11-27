

using System.Text.Json.Serialization;

namespace A2.DTOs
{
    // Esta estrutura é necessária para desserializar a resposta da AwesomeAPI
    public class AwesomeApiRateResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("codein")]
        public string CodeIn { get; set; } = string.Empty;

        // A taxa de venda (Sell Rate), que geralmente usamos em contabilidade.
        [JsonPropertyName("bid")]
        public string Bid { get; set; } = string.Empty;
    }
}