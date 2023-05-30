using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TradeOnAnalysis.Assets.MarketAPI.Utils;

namespace TradeOnAnalysis.Assets.MarketAPI.Results
{
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
}
