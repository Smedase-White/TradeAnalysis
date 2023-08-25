namespace TradeAnalysis.Core.MarketAPI;

using static TypeUtils;

public class OperationHistoryRequest : BaseRequest<OperationHistoryResult>
{
    public OperationHistoryRequest(long startTime, long endTime, string apiKey)
        : base("OperationHistory", $"{startTime}", $"{endTime}", $"?key={apiKey}") { }

    public OperationHistoryRequest(DateTime startDate, DateTime endDate, string apiKey)
        : this(GetUnixTime(startDate), GetUnixTime(endDate), apiKey) { }
}