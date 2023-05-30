using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TradeOnAnalysis.Assets.MarketAPI.Requests;
using TradeOnAnalysis.Assets.MarketAPI.Results;
using TradeOnAnalysis.Assets.MarketAPI.Utils;

namespace TradeOnAnalysis.Assets
{
    public class Item
    {
        private readonly static List<Item> _allItems = new();
        public static List<Item> GetAllItems() => _allItems;

        public long ClassId { get; init; }

        public long InstanceId { get; init; }

        public string Name { get; init; }

        public ActionInfo? BuyInfo { get; private set; }

        public ActionInfo? SellInfo { get; private set; }

        public double? AveragePrice { get; private set; }

        public Dictionary<DateTime, ItemDaylyPrices>? History { get; private set; }

        public Item(long classId, long instanceId, string name)
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
            var found = _allItems.Where(item => item.Name == element.MarketHashName);

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
                item = new(Convert.ToInt64(element.ClassId), Convert.ToInt64(element.InstanceId), element.MarketHashName);
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

        public async Task LoadHistory(string apiKey)
        {
            History = new();

            ItemHistoryRequest request = new(ClassId, InstanceId, apiKey);
            HttpStatusCode status = await Task.Run(() => request.ResultMessage.StatusCode);
            if (status != HttpStatusCode.OK)
                return;

            ItemHistoryResult result = request.Result!;
            foreach (ItemHistoryElement historyElement in result.History)
            {
                DateTime date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(historyElement.Time)).DateTime.Date;
                double price = Convert.ToDouble(historyElement.Price);
                if (History.TryGetValue(date, out ItemDaylyPrices? value))
                    value.AddPrice(price);
                else
                    History.Add(date, new(price));
            }
            AveragePrice = Convert.ToDouble(result.Average);
        }
    }

    public class ActionInfo
    {
        public double Price { get; init; }
        public DateTime Date { get; init; }

        public ActionInfo(double price, DateTime date)
        {
            Price = price;
            Date = date;
        }
    }

    public class ItemDaylyPrices
    {
        public List<double> Prices { get; } = new();

        public double AveragePrice
            => Prices.Sum() / Prices.Count;

        public double Count
            => Prices.Count;

        public ItemDaylyPrices(params double[] prices)
        {
            foreach (double price in prices)
                Prices.Add(price);
        }

        public void AddPrice(double price) => Prices.Add(price);
    }
}
