using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TradeOnAnalysis.Assets.MarketAPI.Answer
{
    public class OperationHistoryAnswer : BaseAnswer
    {
        [JsonPropertyName("history")]
        public List<OperationHistoryElement> History { get; set; } = new();
    }
}
