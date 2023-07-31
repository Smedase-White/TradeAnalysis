using System.Collections.Immutable;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public interface PeriodicityStatistics<StatisticType> : IStatistics where StatisticType : StatisticElement, new()
{
    public IImmutableList<StatisticType>? HourlyData { get; }
    public IImmutableList<StatisticType>? DailyData { get; set; }
    public IImmutableList<StatisticType>? WeeklyData { get; set; }
    public IImmutableList<StatisticType>? MonthlyData { get; set; }

    public void CalcPereodicityStatistics()
    {
        CalcHourlyData();
        CalcDailyData();
        CalcWeeklyData();
        CalcMonthlyData();
    }

    public void CalcHourlyData();

    public void CalcDailyData()
    {
        DailyData = CalcPeriodData(IsEndOfDay)?.ToImmutableList();
    }

    public void CalcWeeklyData()
    {
        WeeklyData = CalcPeriodData(time => IsEndOfDay(time) && IsEndofWeek(time))?.ToImmutableList();
    }

    public void CalcMonthlyData()
    {
        MonthlyData = CalcPeriodData(time => IsEndOfDay(time) && IsEndofMonth(time))?.ToImmutableList();
    }

    public static readonly Func<DateTime, bool> IsEndOfDay = time => time.Hour == 23;

    public static readonly Func<DateTime, bool> IsEndofWeek = time => time.DayOfWeek == DayOfWeek.Sunday;

    public static readonly Func<DateTime, bool> IsEndofMonth = time => time.Day == DateTime.DaysInMonth(time.Year, time.Month);

    public static StatisticType CreatePeriodElement(IEnumerable<StatisticType> periodElements)
    {
        StatisticType element = new();
        element.Combine(CombineType.Sum, periodElements);
        if (element.Time.Hour != 23)
            element.Time = element.Time.AddHours(23 - element.Time.Hour);
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
                periodData.Add(CreatePeriodElement(periodElements));
                periodElements = new();
            }
        }
        if (periodElements.Count != 0)
            periodData.Add(CreatePeriodElement(periodElements));

        return periodData;
    }
}
