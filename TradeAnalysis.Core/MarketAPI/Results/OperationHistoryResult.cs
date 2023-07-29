using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

public class OperationHistoryResult : BaseResult
{
    [JsonPropertyName("history")]
    public List<OperationHistoryElement> History { get; set; } = new();
}