using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TradeOnAnalysis.Assets.MarketAPI.Utils
{
    class ItemHistoryElement
    {
        [JsonPropertyName("l_price")]
        public string Price { get; set; }

        [JsonPropertyName("l_time")]
        public string Time { get; set; }
    }
}
