using System.Collections.Immutable;
using System.Net;

using TradeOnAnalysis.Core.MarketAPI;
using TradeOnAnalysis.Core.Utils.Statistics.Elements;

namespace TradeOnAnalysis.Core.Utils.Statistics;

public class AccountStatistics : FullStatistics<TradeStatisticElement>
{
    public const int MaxItemHistory = 10;

    public string MarketApi { get; init; }

    public IImmutableList<Item>? History { get; private set; }
    public IImmutableList<Item>? TradeHistory { get; private set; }

    public AccountStatistics(string marketApi)
    {
        MarketApi = marketApi;
    }

    private readonly DateTime StartDate = new(2001, 1, 1);
    public HttpStatusCode LoadHistory()
    {
        OperationHistoryRequest request = new(StartDate, DateTime.Today, MarketApi);
        HttpStatusCode status = request.ResultMessage.StatusCode;

        if (status != HttpStatusCode.OK)
            return status;

        OperationHistoryResult result = request.Result!;

        List<Item> history = new();
        foreach (OperationHistoryElement element in result.History)
        {
            if (element.TradeStage == TradeStage.TimedOut)
                continue;
            if (element.EventType == EventType.Transaction)
                continue;
            history.Add(Item.LoadFromAPI(element));
        }
        history.Reverse();

        History = history.ToImmutableList();
        TradeHistory = GetTradeHistory(history).ToImmutableList();

        return status;
    }

    private static List<Item> GetTradeHistory(List<Item> history)
    {
        List<Item> tradeHistory = new(history);

        for (int i = 0; i < tradeHistory.Count; i++)
        {
            if (tradeHistory[i].Name.Contains("Sealed Graffiti"))
            {
                tradeHistory.RemoveAt(i);
                i--;
                continue;
            }

            if (tradeHistory[i].BuyInfo is null)
            {
                tradeHistory.RemoveAt(i);
                i--;
                continue;
            }

            for (int j = i + 1; j < tradeHistory.Count; j++)
            {
                if (tradeHistory[i].Name != tradeHistory[j].Name)
                    continue;

                if (tradeHistory[j].SellInfo is null)
                    continue;

                tradeHistory[i].SellInfo = tradeHistory[j].SellInfo;
                tradeHistory[i].Profit = new(tradeHistory[i].BuyInfo!, tradeHistory[i].SellInfo!);
                tradeHistory.RemoveAt(j);
                break;
            }

            if (tradeHistory[i].SellInfo is null)
            {
                tradeHistory.RemoveAt(i);
                i--;
                continue;
            }
        }

        return tradeHistory;
    }

    private static DateTime GetHistoryDate(Item item)
        => item.BuyInfo?.Date ?? item.SellInfo!.Date;
    private static IEnumerable<DateTime> DaysInRangeUntil(DateTime startDate, DateTime endDate)
    {
        return Enumerable.Range(0, 1 + (int)(endDate - startDate).TotalDays)
                         .Select(dt => startDate.AddDays(dt));
    }
    private static void CalcDailyValues(IEnumerable<TradeStatisticElement> dailyData, IEnumerable<Item> items,
        Func<Item, DateTime, bool> predicate,
        Func<Item, DateTime, double> selection,
        Action<TradeStatisticElement, double> action,
        Func<IEnumerable<double>, double>? calc = null)
    {
        foreach (TradeStatisticElement data in dailyData)
        {
            IEnumerable<double> dayValues = from item in items
                                            where predicate(item, data.Date)
                                            select selection(item, data.Date);
            double value = calc is null ? dayValues.Sum() : calc(dayValues);
            action(data, value);
        }
    }

    public override void CalcDailyData()
    {
        if (History is null || History.Count == 0)
            return;
        DateTime startDate = GetHistoryDate(History![0]), endDate = GetHistoryDate(History![^1]);
        List<TradeStatisticElement> dailyData = (from day in DaysInRangeUntil(startDate, endDate)
                                                 select new TradeStatisticElement() { Date = day }).ToList();

        CalcDailyValues(dailyData, History,
            (item, date) => date == item.BuyInfo?.Date,
            (item, _) => item.BuyInfo!.Price,
            (dayData, value) => dayData.Buy = value);

        CalcDailyValues(dailyData, History,
            (item, date) => date == item.SellInfo?.Date,
            (item, _) => item.SellInfo!.Price,
            (dayData, value) => dayData.Sell = value);

        CalcDailyValues(dailyData, TradeHistory!,
            (item, date) => date == item.SellInfo!.Date,
            (item, _) => item.Profit!.Value,
            (dayData, value) => dayData.Profit = value);

        CalcDailyValues(dailyData, TradeHistory!,
            (item, date) => item.BuyInfo!.Date < date && date <= item.SellInfo!.Date,
            (item, _) => item.Profit!.Daily,
            (dayData, value) => dayData.DailyProfit = value);

        CalcDailyValues(dailyData, History,
            (item, date) => item.History?.ContainsKey(date) ?? false,
            (item, date) => item.History![date].AveragePrice / item.AveragePrice!.Value,
            (dayData, value) => dayData.MarketAnalisys = value,
            (enumerable) => enumerable.Any() ? enumerable.Sum() / enumerable.Count() : 1);

        DailyData = dailyData.ToImmutableList();

    }

    public void CalcData()
    {
        CalcDailyData();
        CalcWeeklyData();
        CalcMonthlyData();
    }

    public async Task ParseMarket()
    {
        List<Task> tasks = new();
        int count = 0;
        foreach (Item item in History!)
        {
            tasks.Add(item.LoadHistory(MarketApi));
            if (++count >= MaxItemHistory)
                break;
        }
        await Task.WhenAll(tasks);
    }
}
