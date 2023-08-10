using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;

namespace TradeAnalysis.Core.Utils.Item;

public class MarketItem
{
    public long? ClassId { get; init; }

    public long? InstanceId { get; init; }

    public string Name { get; init; }

    public ActionInfo? BuyInfo { get; set; }

    public ActionInfo? SellInfo { get; set; }

    public ProfitInfo? Profit { get; set; }

    public double? AveragePrice { get; set; }

    public IImmutableList<OperationInfo>? History { get; set; }

    public MarketItem(long classId, long instanceId, string name)
    {
        ClassId = classId;
        InstanceId = instanceId;
        Name = name;
    }

    public MarketItem(MarketItem item)
    {
        ClassId = item.ClassId;
        InstanceId = item.InstanceId;
        Name = item.Name;
        BuyInfo = item.BuyInfo;
        SellInfo = item.SellInfo;
        Profit = item.Profit;
        AveragePrice = item.AveragePrice;
        History = item.History;
    }

    public static MarketItem LoadFromAPI(OperationHistoryElement element)
    {
        MarketItem item = new(Convert.ToInt64(element.ClassId), Convert.ToInt64(element.InstanceId),
            element.MarketHashName ?? "-");

        if (element.Event == EventType.Buy)
            item.BuyInfo = new ActionInfo(Convert.ToInt32(element.Paid) / 100.0, element.Time);
        else if (element.Event == EventType.Sell)
            item.SellInfo = new ActionInfo(Convert.ToInt32(element.Recieved) / 100.0, element.Time);

        return item;
    }

    public async Task LoadHistory(string apiKey)
    {
        if (ClassId is null || InstanceId is null)
            return;

        ItemHistoryRequest request = new(ClassId ?? 0, InstanceId ?? 0, apiKey);
        HttpStatusCode status = request.ResultMessage.StatusCode;
        int tries = 1;
        //HttpStatusCode status = await Task.Run(() => request.ResultMessage.StatusCode);
        while (status != HttpStatusCode.OK)
        {
            request = new(ClassId ?? 0, InstanceId ?? 0, apiKey);
            status = request.ResultMessage.StatusCode;
            tries++;
            if (tries >= 5)
                break;
        }

        if (status != HttpStatusCode.OK)
            return;

        List<OperationInfo> history = new();
        ItemHistoryResult result = request.Result!;
        AveragePrice = Convert.ToDouble(result.Average);
        foreach (ItemHistoryElement historyElement in result.History)
            history.Add(new()
            {
                Time = historyElement.Time,
                Price = Convert.ToDouble(historyElement.Price) / AveragePrice.Value,
            });

        for (int i = 1; i < history.Count - 1; i++)
        {
            double avg = (history[i - 1].Price + history[i + 1].Price) / 2;
            if (history[i].Price < avg / 1.125 || history[i].Price > avg * 1.125)
            {
                history.RemoveAt(i);
                i--;
            }
        }

        History = history.ToImmutableList();
    }
}