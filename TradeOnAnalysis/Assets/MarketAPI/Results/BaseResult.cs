using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Assets.MarketAPI.Results
{
    public class BaseResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
