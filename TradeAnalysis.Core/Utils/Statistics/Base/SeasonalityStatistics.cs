using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public interface SeasonalityStatistics<StatisticType> : IStatistics where StatisticType : StatisticElement, new()
{
    public IImmutableList<StatisticType>? HourOfDayData { get; set; }
    public IImmutableList<StatisticType>? DayOfWeekData { get; set; }

    public void CalcSeasonalityStatistics()
    {
        CalcHourOfDayData();
        CalcDayOfWeekData();
    }

    public void CalcHourOfDayData();

    public void CalcDayOfWeekData();

    public static StatisticType CreateSeasonElement(IEnumerable<StatisticType> periodElements)
    {
        StatisticType element = new();
        element.Combine(CombineType.Sum, periodElements);
        return element;
    }
}
