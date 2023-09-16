using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

using static StatisticsUtils;

public class Statistics<StatisticType> where StatisticType : StatisticElement, new()
{
    private readonly Period _period;
    private SortedSet<StatisticType>? _data;

    public Statistics(Period period, SortedSet<StatisticType>? data = null)
    {
        _period = period;
        _data = data;
    }

    public Period Period
    {
        get => _period;
    }

    public SortedSet<StatisticType>? Data
    {
        get => _data;
        set => _data = value;
    }

    public virtual void CalcStatistics() { }

    public Statistics<StatisticType>? SelectDataPeriod(DateTime startTime, DateTime endTime)
    {
        return new(_period, SelectDataPeriod<StatisticType>(Data, startTime, endTime));
    }

    public Statistics<StatisticType>? CalcPeriodData(Period period)
    {
        if (period == _period)
            return this;

        return new(period, CalcPeriodData<StatisticType>(Data, period));
    }

    public Statistics<StatisticType>? CalcSeasonData(Period season)
    {
        return new(season.GetSeasonDuration(), CalcSeasonData<StatisticType>(Data, season));
    }

    public void FillStatisticValues<ItemsType>(IEnumerable<ItemsType> items,
        Func<ItemsType, (DateTime, DateTime)> intervalSelection, Func<ItemsType, DateTime, double> valueSelection,
        Action<StatisticType, IEnumerable<double>> action)
    {
        FillStatisticValues<StatisticType, ItemsType>(Data, items, intervalSelection, valueSelection, action);
    }
}
