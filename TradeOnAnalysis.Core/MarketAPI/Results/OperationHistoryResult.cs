using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Core.MarketAPI;

public class OperationHistoryResult : BaseResult
{
    [JsonPropertyName("history")]
    public List<OperationHistoryElement> History { get; set; } = new();
}