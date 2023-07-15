using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Core.MarketAPI
{
    public class BaseResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
