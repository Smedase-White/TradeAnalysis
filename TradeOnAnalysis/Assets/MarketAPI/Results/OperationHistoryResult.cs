using System.Collections.Generic;
using System.Text.Json.Serialization;
using TradeOnAnalysis.Assets.MarketAPI.Utils;

namespace TradeOnAnalysis.Assets.MarketAPI.Results
{
    public class OperationHistoryResult : BaseResult
    {
        [JsonPropertyName("history")]
        public List<OperationHistoryElement> History { get; set; } = new();
    }
}
