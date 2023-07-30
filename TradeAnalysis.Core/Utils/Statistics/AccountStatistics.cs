using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics;

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

    private readonly DateTime StartTime = new(2001, 1, 1, 0, 0, 0);
    public HttpStatusCode LoadHistory()
    {
        OperationHistoryRequest request = new(StartTime, DateTime.Now, MarketApi);
        HttpStatusCode status = request.ResultMessage.StatusCode;

        if (status != HttpStatusCode.OK)
            return status;

        OperationHistoryResult result = request.Result!;

        List<Item> history = new();
        foreach (OperationHistoryElement element in result.History)
        {
            if (element.Stage == TradeStage.TimedOut)
                continue;
            if (element.Event == EventType.Transaction)
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

                tradeHistory[i] = new(tradeHistory[i]);
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



    private static DateTime GetHistoryTime(Item item)
    {
        DateTime rawTime = item.BuyInfo?.Time ?? item.SellInfo!.Time;
        return new(rawTime.Year, rawTime.Month, rawTime.Day, rawTime.Hour, 0, 0);
    }
    private static IEnumerable<DateTime> HoursInRangeUntil(DateTime startTime, DateTime endTime)
    {
        return Enumerable.Range(0, 2 + (int)(endTime - startTime).TotalHours)
                         .Select(time => startTime.AddHours(time));
    }
    private static void CalcHourlyValues(IEnumerable<TradeStatisticElement> hourlyData, IEnumerable<Item> items,
        Func<Item, DateTime, bool> predicate,
        Func<Item, DateTime, double> selection,
        Action<TradeStatisticElement, double> action,
        Func<IEnumerable<double>, double>? calc = null)
    {
        foreach (TradeStatisticElement data in hourlyData)
        {
            IEnumerable<double> hourValues = from item in items
                                             where predicate(item, data.Time)
                                             select selection(item, data.Time);
            double value = calc is null ? hourValues.Sum() : calc(hourValues);
            action(data, value);
        }
    }

    private bool TimeEquals(DateTime? a, DateTime? b)
    {
        if (a is null || b is null)
            return false;
        return a.Value.Year == b.Value.Year && a.Value.Month == b.Value.Month 
            && a.Value.Day == b.Value.Day && a.Value.Hour == b.Value.Hour;
    }
    public override void CalcHourlyData()
    {
        if (History is null || History.Count == 0)
            return;
        DateTime startTime = GetHistoryTime(History![0]), endTime = GetHistoryTime(History![^1]);
        List<TradeStatisticElement> hourlyData = (from time in HoursInRangeUntil(startTime, endTime)
                                                  select new TradeStatisticElement() { Time = time }).ToList();

        CalcHourlyValues(hourlyData, History,
            (item, date) => TimeEquals(date, item.BuyInfo?.Time),
            (item, _) => item.BuyInfo!.Price,
            (hourData, value) => hourData.Buy = value);

        CalcHourlyValues(hourlyData, History,
            (item, date) => TimeEquals(date, item.SellInfo?.Time),
            (item, _) => item.SellInfo!.Price,
            (hourData, value) => hourData.Sell = value);

        CalcHourlyValues(hourlyData, TradeHistory!,
            (item, date) => TimeEquals(date, item.SellInfo?.Time),
            (item, _) => item.Profit!.Value,
            (hourData, value) => hourData.Profit = value);

        CalcHourlyValues(hourlyData, TradeHistory!,
            (item, date) => item.BuyInfo!.Time < date && date <= item.SellInfo!.Time,
            (item, _) => item.Profit!.Hourly,
            (hourData, value) => hourData.HourlyProfit = value);

        HourlyData = hourlyData.ToImmutableList();
    }
}
