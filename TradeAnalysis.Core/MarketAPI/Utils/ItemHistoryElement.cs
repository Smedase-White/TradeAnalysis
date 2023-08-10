using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

class ItemHistoryElement
{
    [JsonPropertyName("l_price")]
    public string? Price { get; set; }

    [JsonPropertyName("l_time")]
    public string? TimeString { get; set; }
    public DateTime Time
        => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(TimeString)).LocalDateTime;
}