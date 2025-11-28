

using System.Text.Json.Serialization;

namespace A2.DTOs
{
    public class AwesomeApiRateResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("codein")]
        public string CodeIn { get; set; } = string.Empty;

        [JsonPropertyName("bid")]
        public string Bid { get; set; } = string.Empty;
    }
}