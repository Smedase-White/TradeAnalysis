using System.Reflection;

namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement : IComparable<StatisticElement>
{
    [Combinable(CalculationType.Last)]
    public DateTime Time { get; set; }

    public StatisticElement? Prev { get; set; }

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
        double defaultValue = selector(this);
        return elements.Where(e => selector(e) != defaultValue).Sum(selector);
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
