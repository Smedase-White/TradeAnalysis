using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Assets.MarketAPI.Answer
{
    public class BaseAnswer
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
