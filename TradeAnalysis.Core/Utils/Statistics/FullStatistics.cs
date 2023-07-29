using System.Collections.Immutable;

using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics;

public abstract class FullStatistics<StatisticType> where StatisticType : StatisticElement, new()
{
    public IImmutableList<StatisticType>? DailyData { get; set; }
    public IImmutableList<StatisticType>? WeeklyData { get; set; }
    public IImmutableList<StatisticType>? MonthlyData { get; set; }

    public abstract void CalcDailyData();
    public virtual void CalcWeeklyData()
    {
        WeeklyData = CalcPeriodData(date => date.DayOfWeek == DayOfWeek.Sunday)?.ToImmutableList();
    }
    public virtual void CalcMonthlyData()
    {
        MonthlyData = CalcPeriodData(date => date.Day == DateTime.DaysInMonth(date.Year, date.Month))?.ToImmutableList();
    }

    private StatisticType CreateElement(List<StatisticType> periodElements)
    {
        StatisticType element = new();
        element.Combine(CombineType.Sum, periodElements);
        return element;
    }
    public List<StatisticType>? CalcPeriodData(Func<DateTime, bool> point)
    {
        if (DailyData is null)
            return null;

        List<StatisticType> periodData = new();
        List<StatisticType> periodElements = new();
        foreach (StatisticType data in DailyData)
        {
            periodElements.Add(data);
            if (point(data.Date) == true)
            {
                periodData.Add(CreateElement(periodElements));
                periodElements = new();
            }
        }
        if (periodElements.Count != 0)
            periodData.Add(CreateElement(periodElements));

        return periodData;
    }
}
