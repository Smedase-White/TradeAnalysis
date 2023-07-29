using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

public class BaseResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}