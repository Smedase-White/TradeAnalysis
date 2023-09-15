using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public static class StatisticsUtils
{
    public static SortedSet<StatisticType>? SelectDataPeriod<StatisticType>(
        SortedSet<StatisticType>? data, DateTime startTime, DateTime endTime)
        where StatisticType : StatisticElement, new()
    {
        if (startTime > data?.Max?.Time || endTime < data?.Min?.Time)
            return new();

        if (startTime < data?.Min?.Time)
            startTime = data.Min.Time;
        if (endTime > data?.Max?.Time)
            endTime = data.Max.Time;

        StatisticType start = new() { Time = startTime }, end = new() { Time = endTime };
        return data?.GetViewBetween(start, end);
    }

    public static SortedSet<StatisticType> CalcPeriodData<StatisticType>(
        SortedSet<StatisticType>? data, Period period)
        where StatisticType : StatisticElement, new()
    {
        if (data is null || data.Any() == false)
            return new();

        SortedSet<StatisticType> periodData = new();
        ICollection<DateTime> times = GetTimeCollection(data.Min!.Time, data.Max!.Time, period);
        StatisticType? statistic = null;
        foreach (DateTime time in times)
        {
            statistic = StatisticElement.Create(
                SelectDataPeriod(data, time.Floor(period), time.Ceiling(period))!, statistic);
            if (statistic is null)
                continue;
            statistic.Time = time.Ceiling(period);
            periodData.Add(statistic);
        }
        return periodData;
    }

    public static SortedSet<StatisticType> CalcSeasonData<StatisticType>(
        SortedSet<StatisticType>? data, Period season)
        where StatisticType : StatisticElement, new()
    {
        if (data is null || data.Any() == false)
            return new();

        SortedSet<StatisticType> seasonData = new();
        SortedList<DateTime, List<StatisticType>> times
            = new(GetTimeCollection(SeasonTime, SeasonTime.Ceiling(season), season.GetSeasonDuration())
            .ToDictionary(time => time, _ => new List<StatisticType>()));
        foreach (StatisticType element in data)
            times[element.Time.ToSeasonTime(season)].Add(element);
        StatisticType? statistic = null;
        foreach (KeyValuePair<DateTime, List<StatisticType>> time in times)
        {
            statistic = StatisticElement.Create(time.Value, statistic);
            if (statistic is null)
                continue;
            statistic.Time = time.Key;
            seasonData.Add(statistic);
        }

        return seasonData;
    }

    public static void FillStatisticValues<StatisticType, ItemsType>(
        SortedSet<StatisticType>? data, IEnumerable<ItemsType> items,
        Func<ItemsType, (DateTime, DateTime)> intervalSelection, Func<ItemsType, DateTime, double> valueSelection,
        Action<StatisticType, IEnumerable<double>> action)
        where StatisticType : StatisticElement, new()
    {
        SortedList<StatisticType, List<double>> selectedTimes = new();
        foreach (ItemsType item in items)
        {
            (DateTime, DateTime) interval = intervalSelection(item);
            if (interval.Equals(NullTime))
                continue;
            SortedSet<StatisticType>? periodSet = SelectDataPeriod(data, interval.Item1, interval.Item2);
            if (periodSet is null)
                continue;
            foreach (StatisticType element in periodSet)
            {
                if (selectedTimes.TryGetValue(element, out List<double>? list) == false)
                {
                    list = new List<double>();
                    selectedTimes.Add(element, list);
                }
                list.Add(valueSelection(item, element.Time));
            }
        }
        foreach (KeyValuePair<StatisticType, List<double>> value in selectedTimes)
            action(value.Key, value.Value);
    }
}
