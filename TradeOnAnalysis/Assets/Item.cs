using System;
using System.Collections.Generic;
using System.Linq;
using TradeOnAnalysis.Assets.MarketAPI;

namespace TradeOnAnalysis.Assets
{
    public class ActionInfo
    {
        public double Price { get; init; }
        public DateTime Date { get; init; }

        public ActionInfo(double price, DateTime date)
        {
            Price = price;
            Date = date;
        }

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} | {Price}";
        }
    }

    public class Item
    {
        private readonly static List<Item> _allItems = new();
        public static List<Item> GetAllItems() => _allItems;

        public string ClassId { get; init; }

        public string InstanceId { get; init; }

        public string Name { get; init; }

        public ActionInfo? BuyInfo { get; private set; }

        public ActionInfo? SellInfo { get; private set; }

        public Item(string classId, string instanceId, string name)
        {
            ClassId = classId;
            InstanceId = instanceId;
            Name = name;
        }

        public static Item? LoadFromAPI(OperationHistoryElement element)
        {
            if (element.EventType == EventType.Transaction)
                return null;

            if (element.TradeStage == TradeStage.TimedOut)
                return null;

            if (element.MarketHashName.Contains("Sealed Graffiti"))
                return null;

            Item? item;
            var found = from x in _allItems
                        where x.Name == element.MarketHashName
                        select x;

            switch (element.EventType)
            {
                case EventType.Buy:
                    found = found.Where(x => x.BuyInfo == null);
                    break;
                case EventType.Sell:
                    found = found.Where(x => x.SellInfo == null);
                    break;
            }

            if (found.Any())
            {
                item = found.First();
            }
            else
            {
                item = new(element.ClassId, element.InstanceId, element.MarketHashName);
                _allItems.Add(item);
            }

            switch (element.EventType)
            {
                case EventType.Buy:
                    item.BuyInfo = new ActionInfo(Convert.ToInt32(element.Paid) / 100.0, element.DateTime);
                    break;
                case EventType.Sell:
                    item.SellInfo = new ActionInfo(Convert.ToInt32(element.Recieved) / 100.0, element.DateTime); ;
                    break;
            }

            return item;
        }
    }
}
