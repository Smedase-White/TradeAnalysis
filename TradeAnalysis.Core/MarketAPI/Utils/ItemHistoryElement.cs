using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

class ItemHistoryElement
{
    [JsonPropertyName("l_price")]
    public string? Price { get; set; }

    [JsonPropertyName("l_time")]
    public string? Time { get; set; }
}