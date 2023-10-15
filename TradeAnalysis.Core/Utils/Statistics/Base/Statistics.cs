using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.Core.Utils.Statistics.Base;

public class Statistics<StatisticType> where StatisticType : StatisticElement, new()
{
    private readonly Period _timeStep;
    private DateTime _startTime;
    private StatisticType[] _data;

    public Statistics(Period timeStep, IEnumerable<StatisticType>? data = null)
    {
        _timeStep = timeStep;
        if (data is not null)
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

    public Period TimeStep
    {
        get => _timeStep;
    }

    public DateTime StartTime
    {
        get => _startTime;
    }

    public StatisticType[] Data
    {
        get => _data;
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
        return (int)(dateTime - StartTime).Length(TimeStep);
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
        return StartTime.AddPeriod(TimeStep, index);
    }

    public StatisticType[] CreateData(DateTime startTime, DateTime endTime)
    {
        _startTime = startTime;
        _data = GetTimeEnumerable(startTime, endTime, _timeStep)
            .Select(time => new StatisticType() { Time = time }).ToArray();
        return _data;
    }

    public virtual void CalcStatistics() { }

    public Statistics<StatisticType> SelectPeriod(DateTime startTime, DateTime endTime)
    {
        Range? range = GetRange(startTime, endTime);
        if (range.HasValue == false)
            return new(_timeStep);
        return new(_timeStep, Data[range.Value]);
    }

    public Statistics<StatisticType> CalcPeriodStatistics(Period timeStep)
    {
        if (timeStep == _timeStep)
            return this;

        if (Data.Length == 0)
            return new(timeStep);

        List<StatisticType> periodData = new();
        StatisticType? statistic = null;
        foreach (DateTime time in GetTimeEnumerable(Data[0].Time, Data[^1].Time, timeStep))
        {
            DateTime ceilingTime = time.Ceiling(timeStep);
            statistic = StatisticElement.Create(SelectPeriod(time, ceilingTime).Data, statistic);
            if (statistic is null)
                continue;
            statistic.Time = ceilingTime;
            periodData.Add(statistic);
        }

        return new(timeStep, periodData);
    }

    public Statistics<StatisticType> CalcSeasonStatistics(Period seasonStep)
    {
        if (Data.Length == 0)
            return new(seasonStep.GetSeasonDuration());

        SortedList<DateTime, List<StatisticType>> times
            = new(GetTimeEnumerable(SeasonTime, SeasonTime.Ceiling(seasonStep), seasonStep.GetSeasonDuration())
            .ToDictionary(time => time, _ => new List<StatisticType>()));
        foreach (StatisticType element in Data)
            times[element.Time.ToSeasonTime(seasonStep)].Add(element);

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

        return new(seasonStep.GetSeasonDuration(), seasonData);
    }

    public void FillStatisticValues<ItemsType>(IEnumerable<ItemsType> items,
        Func<ItemsType, (DateTime, DateTime)> intervalSelection, Func<ItemsType, DateTime, double> valueSelection,
        Action<StatisticType, List<double>> action)
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
