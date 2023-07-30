﻿using System.Net;

using TradeAnalysis.Core.MarketAPI;

namespace TradeAnalysis.Core.Utils;

public class Item
{
    public long? ClassId { get; init; }

    public long? InstanceId { get; init; }

    public string Name { get; init; }

    public ActionInfo? BuyInfo { get; set; }

    public ActionInfo? SellInfo { get; set; }

    public ProfitInfo? Profit { get; set; }

    public double? AveragePrice { get; set; }

    public Dictionary<DateTime, ItemDailyPrices>? History { get; set; }

    public Item(long classId, long instanceId, string name)
    {
        ClassId = classId;
        InstanceId = instanceId;
        Name = name;
    }

    public Item(Item item)
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

    public static Item LoadFromAPI(OperationHistoryElement element)
    {
        Item item = new(Convert.ToInt64(element.ClassId), Convert.ToInt64(element.InstanceId),
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

        History = new();

        ItemHistoryRequest request = new(ClassId ?? 0, InstanceId ?? 0, apiKey);
        HttpStatusCode status = await Task.Run(() => request.ResultMessage.StatusCode);
        if (status != HttpStatusCode.OK)
            return;

        ItemHistoryResult result = request.Result!;
        foreach (ItemHistoryElement historyElement in result.History)
        {
            DateTime date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(historyElement.Time)).DateTime.Date;
            double price = Convert.ToDouble(historyElement.Price);
            if (History.TryGetValue(date, out ItemDailyPrices? value))
                value.AddPrice(price);
            else
                History.Add(date, new(price));
        }
        AveragePrice = Convert.ToDouble(result.Average);
    }
}