namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement : IComparable<StatisticElement>
{
    private DateTime _time;

    public DateTime Time
    {
        get => _time;
        set => _time = value;
    }

    public virtual bool IsEmpty
        => true;

    public virtual void Combine<StatisticType>(IEnumerable<StatisticType> elements)
        where StatisticType : StatisticElement, new()
    {
        DateTime maxTime = elements.OrderBy(e => e.Time).Last().Time;
        if (maxTime > Time)
            Time = maxTime;
    }

    public static void Sum<StatisticType>(ref double property,
        IEnumerable<StatisticType> values, Func<StatisticType, double> selector)
        where StatisticType : StatisticElement, new()
    {
        property += values.Sum(selector);
    }

    public static void Average<StatisticType>(ref double property,
        IEnumerable<StatisticType> values, Func<StatisticType, double> selector)
        where StatisticType : StatisticElement, new()
    {
        property = (property + values.Sum(selector)) / (1 + values.Count());
    }

    public static StatisticType? Create<StatisticType>(IEnumerable<StatisticElement> elements)
        where StatisticType : StatisticElement, new()
    {
        if (elements.Any() == false)
            return null;

        StatisticType element = new();
        element.Combine(elements);
        return element;
    }

    public int CompareTo(StatisticElement? other)
    {
        return Time.CompareTo(other?.Time);
    }
}
