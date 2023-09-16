using System.Reflection;

namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement : IComparable<StatisticElement>
{
    private DateTime _time;
    private StatisticElement? _prev;

    [Combinable(CalculationType.Last)]
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

    public void Combine(IEnumerable<StatisticElement> elements)
    {
        foreach (PropertyInfo property in GetType().GetProperties())
        {
            CombinableAttribute? attribute = property.GetCustomAttribute<CombinableAttribute>();
            if (attribute is null)
                continue;
            switch (attribute.CalculationType)
            {
                case CalculationType.Sum:
                    property.SetValue(this, Sum(elements, element => (double)property.GetValue(element)!));
                    break;
                case CalculationType.Avg:
                    property.SetValue(this, Average(elements, element => (double)property.GetValue(element)!));
                    break;
                case CalculationType.Last:
                    property.SetValue(this, property.GetValue(elements.Last()));
                    break;
            }
        }
    }

    public double Sum(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selector)
    {
        return elements.Sum(selector);
    }

    public double Average(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selector)
    {
        double defaultValue = selector(this);
        elements = elements.Where(e => selector(e) != defaultValue);
        if (elements.Any() == false)
            return defaultValue;

        return elements.Sum(selector) / elements.Count();
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
