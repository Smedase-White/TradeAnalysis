using System.Net;
using TradeOnAnalysis.Core.MarketAPI;

namespace TradeOnAnalysis.Core.Utils;

public class Item
{
    public long Id { get; init; }

    public string Name { get; init; }

    public long? ClassId { get; private set; }

    public long? InstanceId { get; private set; }

    public ActionInfo? BuyInfo { get; set; }

    public ActionInfo? SellInfo { get; set; }

    public double? AveragePrice { get; set; }

    public Dictionary<DateTime, ItemDailyPrices>? History { get; set; }

    public Item(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Item LoadFromAPI(OperationHistoryElement element)
    {
        Item item = new(Convert.ToInt64(element.Id), element.MarketHashName ?? "-")
        {
            ClassId = Convert.ToInt64(element.ClassId),
            InstanceId = Convert.ToInt64(element.InstanceId)
        };

        if (element.EventType == EventType.Buy)
            item.BuyInfo = new ActionInfo(Convert.ToInt32(element.Paid) / 100.0, element.DateTime);
        else if (element.EventType == EventType.Sell)
            item.SellInfo = new ActionInfo(Convert.ToInt32(element.Recieved) / 100.0, element.DateTime);

        return item;
    }
}