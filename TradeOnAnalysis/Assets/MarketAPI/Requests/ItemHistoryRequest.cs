using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeOnAnalysis.Assets.MarketAPI.Results;

namespace TradeOnAnalysis.Assets.MarketAPI.Requests
{
    class ItemHistoryRequest : BaseRequest<ItemHistoryResult>
    {
        public ItemHistoryRequest(long classId, long instanceId, string apiKey)
            : base("ItemHistory", $"{classId}_{instanceId}", $"?key={apiKey}") { }
    }
}
