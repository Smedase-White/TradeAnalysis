using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

using static TypeUtils;

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