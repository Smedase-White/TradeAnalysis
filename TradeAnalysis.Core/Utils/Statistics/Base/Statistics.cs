using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public class Statistics<StatisticType> where StatisticType : StatisticElement, new()
{
    private readonly Period _period;
    private DateTime _startTime;
    private StatisticType[] _data;

    public Statistics(Period period, IEnumerable<StatisticType>? data = null)
    {
        _period = period;
        if (data != null)
        {
            _startTime = data.First().Time;
            _data = data.ToArray();
        }
        else
        {
            _startTime = DateTime.MinValue;
            _data = Array.Empty<StatisticType>();
        }
    }

    public Period Period
    {
        get => _period;
    }

    public DateTime StartTime
    {
        get => _startTime;
    }

    public StatisticType[] Data
    {
        get => _data;
        set
        {
            _data = value;
            _startTime = _data[0].Time;
        }
    }

    public StatisticType this[int index]
    {
        get => Data[index];
        set => Data[index] = value;
    }

    public StatisticType this[DateTime dateTime]
    {
        get => Data[GetIndex(dateTime)];
        set => Data[GetIndex(dateTime)] = value;
    }

    public int GetIndex(DateTime dateTime)
    {
        return (int)(dateTime - StartTime).Length(Period);
    }

    public Range? GetRange(DateTime startTime, DateTime endTime)
    {
        int startIndex = GetIndex(startTime), endIndex = GetIndex(endTime);
        if (startIndex >= Data.Length || endIndex < 0)
            return null;

        if (startIndex < 0)
            startIndex = 0;
        if (endIndex >= Data.Length)
            endIndex = Data.Length - 1;

        return startIndex..(endIndex + 1);
    }

    public DateTime GetTime(int index)
    {
        return StartTime.AddPeriod(Period, index);
    }

    public virtual void CalcStatistics() { }

    public Statistics<StatisticType> SelectDataPeriod(DateTime startTime, DateTime endTime)
    {
        Range? range = GetRange(startTime, endTime);
        if (range.HasValue == false)
            return new(_period);
        return new(_period, Data[range.Value]);
    }

    public Statistics<StatisticType>? CalcPeriodStatistics(Period period)
    {
        if (period == _period)
            return this;

        if (Data.Length == 0)
            return new(period);

        List<StatisticType> periodData = new();
        StatisticType? statistic = null;
        foreach (DateTime time in GetTimeEnumerable(Data[0].Time, Data[^1].Time, period))
        {
            statistic = StatisticElement.Create(SelectDataPeriod(time, time.Ceiling(period)).Data, statistic);
            if (statistic is not null)
                periodData.Add(statistic);
        }

        return new(period, periodData);
    }

    public Statistics<StatisticType>? CalcSeasonStatistics(Period season)
    {
        if (Data.Length == 0)
            return new(season.GetSeasonDuration());

        SortedList<DateTime, List<StatisticType>> times
            = new(GetTimeEnumerable(SeasonTime, SeasonTime.Ceiling(season), season.GetSeasonDuration())
            .ToDictionary(time => time, _ => new List<StatisticType>()));
        foreach (StatisticType element in Data)
            times[element.Time.ToSeasonTime(season)].Add(element);

        List<StatisticType> seasonData = new();
        StatisticType? statistic = null;
        foreach (KeyValuePair<DateTime, List<StatisticType>> time in times)
        {
            statistic = StatisticElement.Create(time.Value, statistic);
            if (statistic is null)
                continue;
            statistic.Time = time.Key;
            seasonData.Add(statistic);
        }

        return new(season.GetSeasonDuration(), seasonData);
    }

    public void FillStatisticValues<ItemsType>(IEnumerable<ItemsType> items,
        Func<ItemsType, (DateTime, DateTime)> intervalSelection, Func<ItemsType, DateTime, double> valueSelection,
        Action<StatisticType, IEnumerable<double>> action)
    {
        List<double>[] selectedValues = new List<double>[Data.Length];
        for (int i = 0; i < Data.Length; i++)
            selectedValues[i] = new();
        foreach (ItemsType item in items)
        {
            (DateTime, DateTime) interval = intervalSelection(item);
            if (interval.Equals(NullTime))
                continue;

            Range? range = GetRange(interval.Item1, interval.Item2);
            if (range.HasValue == false)
                continue;

            for (int i = range.Value.Start.Value; i < range.Value.End.Value; i++)
                selectedValues[i].Add(valueSelection(item, GetTime(i)));
        }
        for (int i = 0; i < Data.Length; i++)
            action(Data[i], selectedValues[i]);
    }
}
