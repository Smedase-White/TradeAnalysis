using System;
using TradeOnAnalysis.Assets.MarketAPI.Results;

namespace TradeOnAnalysis.Assets.MarketAPI.Requests
{
    public class OperationHistoryRequest : BaseRequest<OperationHistoryResult>
    {
        public OperationHistoryRequest(long startTime, long endTime, string apiKey)
            : base("OperationHistory", $"{startTime}", $"{endTime}", $"?key={apiKey}") { }

        public OperationHistoryRequest(DateTime startDate, DateTime endDate, string apiKey)
            : this(DateTimeToUnix(startDate), DateTimeToUnix(endDate), apiKey) { }

        private static long DateTimeToUnix(DateTime date)
            => ((DateTimeOffset)date).ToUnixTimeSeconds();
    }
}
