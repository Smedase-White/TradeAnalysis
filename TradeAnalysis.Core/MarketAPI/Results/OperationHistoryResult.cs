using System.Text.Json.Serialization;

namespace TradeAnalysis.Core.MarketAPI;

public class OperationHistoryResult : BaseResult
{
    [JsonPropertyName("history")]
    public List<OperationHistoryBase> History { get; set; } = new();
}