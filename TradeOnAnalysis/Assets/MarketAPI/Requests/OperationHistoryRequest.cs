using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TradeOnAnalysis.Assets.MarketAPI.Results;

namespace TradeOnAnalysis.Assets.MarketAPI.Requests
{
    public class OperationHistoryRequest : BaseRequest
    {
        public OperationHistoryRequest(long startTime, long endTime, string apiKey)
            : base("OperationHistory", $"{startTime}", $"{endTime}", $"?key={apiKey}") { }

        public OperationHistoryRequest(DateTime startDate, DateTime endDate, string apiKey)
            : this(DateTimeToUnix(startDate), DateTimeToUnix(endDate), apiKey) { }

        public override BaseResult? Result
            => DeserializeMessage<OperationHistoryResult>(ResultMessage);

        private static long DateTimeToUnix(DateTime date)
            => ((DateTimeOffset)date).ToUnixTimeSeconds();
    }
}
