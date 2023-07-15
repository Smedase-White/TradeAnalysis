namespace TradeOnAnalysis.Core.MarketAPI
{
    class ItemHistoryRequest : BaseRequest<ItemHistoryResult>
    {
        public ItemHistoryRequest(long classId, long instanceId, string apiKey)
            : base("ItemHistory", $"{classId}_{instanceId}", $"?key={apiKey}") { }
    }
}
