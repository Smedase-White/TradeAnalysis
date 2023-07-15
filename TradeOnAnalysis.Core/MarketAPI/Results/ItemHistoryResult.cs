using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Core.MarketAPI;

class ItemHistoryResult : BaseResult
{
    [JsonPropertyName("max")]
    public int Max { get; set; }

    [JsonPropertyName("min")]
    public int Min { get; set; }

    [JsonPropertyName("average")]
    public int Average { get; set; }

    [JsonPropertyName("number")]
    public int Count { get; set; }

    [JsonPropertyName("history")]
    public List<ItemHistoryElement> History { get; set; } = new();
}