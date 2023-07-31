using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics;

public class TradeStatistics : PeriodicityStatistics<TradeStatisticElement>, SeasonalityStatistics<OperationStatisticElement>
{
    private readonly Account _account;

    public IImmutableList<TradeStatisticElement>? HourlyData { get; set; }
    public IImmutableList<TradeStatisticElement>? DailyData { get; set; }
    public IImmutableList<TradeStatisticElement>? WeeklyData { get; set; }
    public IImmutableList<TradeStatisticElement>? MonthlyData { get; set; }

    public IImmutableList<OperationStatisticElement>? HourOfDayData { get; set; }
    public IImmutableList<OperationStatisticElement>? DayOfWeekData { get; set; }

    public TradeStatistics(Account account)
    {
        _account = account;
        CalcStatistics();
    }

    public void CalcStatistics()
    {
        ((PeriodicityStatistics<TradeStatisticElement>)this).CalcPereodicityStatistics();
        ((SeasonalityStatistics<OperationStatisticElement>)this).CalcSeasonalityStatistics();
    }

    public void CalcHourlyData()
    {
        if (_account.History is null || _account.History.Count == 0)
            return;

        DateTime startTime = GetHistoryTime(_account.History![0]), 
            endTime = GetHistoryTime(_account.History![^1]).AddHours(1);
        List<TradeStatisticElement> data = (from time in TimeUtils.HoursInRangeUntil(startTime, endTime)
                                            select new TradeStatisticElement() { Time = time }).ToList();

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => TimeUtils.TimeByHourEquals(date, item.BuyInfo?.Time),
            (item, _) => item.BuyInfo!.Price,
            (data, value) => data.Buy = value);

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => TimeUtils.TimeByHourEquals(date, item.SellInfo?.Time),
            (item, _) => item.SellInfo!.Price,
            (data, value) => data.Sell = value);

        IStatistics.CalcStatisticValues(data, _account.TradeHistory!,
            (item, date) => TimeUtils.TimeByHourEquals(date, item.SellInfo?.Time),
            (item, _) => item.Profit!.Value,
            (data, value) => data.Profit = value);

        IStatistics.CalcStatisticValues(data, _account.TradeHistory!,
            (item, date) => item.BuyInfo!.Time < date && date <= item.SellInfo!.Time,
            (item, _) => item.Profit!.Hourly,
            (data, value) => data.HourlyProfit = value);

        HourlyData = data.ToImmutableList();
    }

    public void CalcHourOfDayData()
    {
        if (_account.History is null || _account.History.Count == 0)
            return;

        DateTime startTime = new(2001, 1, 1, 0, 0, 0), endTime = new(2001, 1, 1, 23, 0, 0);
        List<OperationStatisticElement> data = (from time in TimeUtils.HoursInRangeUntil(startTime, endTime)
                                                select new OperationStatisticElement() { Time = time }).ToList();

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => item.BuyInfo?.Time.Hour == date.Hour,
            (item, _) => item.BuyInfo!.Price,
            (data, value) => data.Buy = value);

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => item.SellInfo?.Time.Hour == date.Hour,
            (item, _) => item.SellInfo!.Price,
            (data, value) => data.Sell = value);

        HourOfDayData = data.ToImmutableList();
    }

    public void CalcDayOfWeekData()
    {
        if (_account.History is null || _account.History.Count == 0)
            return;

        DateTime startTime = new(2001, 1, 1, 0, 0, 0), endTime = new(2001, 1, 7, 0, 0, 0);
        List<OperationStatisticElement> data = (from time in TimeUtils.DaysInRangeUntil(startTime, endTime)
                                                select new OperationStatisticElement() { Time = time }).ToList();

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => item.BuyInfo?.Time.DayOfWeek == date.DayOfWeek,
            (item, _) => item.BuyInfo!.Price,
            (data, value) => data.Buy = value);

        IStatistics.CalcStatisticValues(data, _account.History,
            (item, date) => item.SellInfo?.Time.DayOfWeek == date.DayOfWeek,
            (item, _) => item.SellInfo!.Price,
            (data, value) => data.Sell = value);

        DayOfWeekData = data.ToImmutableList();
    }

    private static DateTime GetHistoryTime(Item item)
    {
        DateTime rawTime = item.BuyInfo?.Time ?? item.SellInfo!.Time;
        return new(rawTime.Year, rawTime.Month, rawTime.Day, rawTime.Hour, 0, 0);
    }
}
