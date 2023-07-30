using System.Collections.Immutable;

using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics;

public abstract class FullStatistics<StatisticType> where StatisticType : StatisticElement, new()
{
    public IImmutableList<StatisticType>? HourlyData { get; set; }
    public IImmutableList<StatisticType>? DailyData { get; set; }
    public IImmutableList<StatisticType>? WeeklyData { get; set; }
    public IImmutableList<StatisticType>? MonthlyData { get; set; }

    public abstract void CalcHourlyData();
    public virtual void CalcDailyData()
    {
        DailyData = CalcPeriodData(IsEndOfDay)?.ToImmutableList();
    }
    public virtual void CalcWeeklyData()
    {
        WeeklyData = CalcPeriodData(time => IsEndOfDay(time) && IsEndofWeek(time))?.ToImmutableList();
    }
    public virtual void CalcMonthlyData()
    {
        MonthlyData = CalcPeriodData(time => IsEndOfDay(time) && IsEndofMonth(time))?.ToImmutableList();
    }

    public void CalcData()
    {
        CalcHourlyData();
        CalcDailyData();
        CalcWeeklyData();
        CalcMonthlyData();
    }

    private StatisticType CreateElement(List<StatisticType> periodElements)
    {
        StatisticType element = new();
        element.Combine(CombineType.Sum, periodElements);
        return element;
    }
    public List<StatisticType>? CalcPeriodData(Func<DateTime, bool> point)
    {
        if (HourlyData is null)
            return null;

        List<StatisticType> periodData = new();
        List<StatisticType> periodElements = new();
        foreach (StatisticType data in HourlyData)
        {
            periodElements.Add(data);
            if (point(data.Time) == true)
            {
                periodData.Add(CreateElement(periodElements));
                periodElements = new();
            }
        }
        if (periodElements.Count != 0)
            periodData.Add(CreateElement(periodElements));

        return periodData;
    }

    private static Func<DateTime, bool> IsEndOfDay
        => time => time.Hour == 23;

    private static Func<DateTime, bool> IsEndofWeek
        => time => time.DayOfWeek == DayOfWeek.Sunday;

    private static Func<DateTime, bool> IsEndofMonth
        => time => time.Day == DateTime.DaysInMonth(time.Year, time.Month);
}
