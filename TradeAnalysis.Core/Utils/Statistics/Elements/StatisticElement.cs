using System.Collections.Generic;

namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement : IComparable<StatisticElement>
{
    private DateTime _time;
    private StatisticElement? _prev;

    public DateTime Time
    {
        get => _time;
        set => _time = value;
    }

    public StatisticElement? Prev
    {
        get => _prev;
        set => _prev = value;
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

    public static void Sum<StatisticType>(ref double? property,
        IEnumerable<StatisticType> elements, Func<StatisticType, double?> selector)
        where StatisticType : StatisticElement, new()
    {
        property = property.GetValueOrDefault() + elements.Sum(selector);
    }

    public static void Average<StatisticType>(ref double? property,
        IEnumerable<StatisticType> elements, Func<StatisticType, double?> selector)
        where StatisticType : StatisticElement, new()
    {
        elements = elements.Where(e => selector(e) is not null);
        if (elements.Any() == false)
            return;

        if (property is null)
            property = elements.Sum(selector) / elements.Count();
        else
            property = (property + elements.Sum(selector)) / (1 + elements.Count());
    }

    public static StatisticType? Create<StatisticType>(IEnumerable<StatisticType> elements, StatisticType? prev = null)
        where StatisticType : StatisticElement, new()
    {
        if (elements.Any() == false)
            return null;

        StatisticType element = new();
        element.Combine(elements);
        element.Prev = prev;
        return element;
    }

    public int CompareTo(StatisticElement? other)
    {
        return Time.CompareTo(other?.Time);
    }
}
