using System.Text.Json.Serialization;

using static TradeAnalysis.Core.MarketAPI.TypeUtils;

namespace TradeAnalysis.Core.MarketAPI;

public class ItemHistoryElement
{
    [JsonPropertyName("l_price")]
    public string? PriceString { get; set; }
    public long? Price
        => GetLong(PriceString);

    [JsonPropertyName("l_time")]
    public string? TimeString { get; set; }
    public DateTime? Time
        => GetTime(TimeString);
}