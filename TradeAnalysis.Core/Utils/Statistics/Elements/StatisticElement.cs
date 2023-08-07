namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement : IComparable<StatisticElement>
{
    public DateTime Time { get; set; }

    public virtual bool IsEmpty
        => true;

    public virtual void Combine(IEnumerable<StatisticElement> elements, CombineType combineType)
    {
        switch (combineType)
        {
            case CombineType.Sum:
                DateTime maxTime = elements.OrderBy(e => e.Time).Last().Time;
                if (maxTime > Time)
                    Time = maxTime;
                break;
            case CombineType.Average:
                Time.AddHours(elements.Sum(e => (e.Time - Time).Hours) / (elements.Count() + 1));
                break;
            default:
                break;
        }
    }

    public double GetSum(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selection)
    {
        return selection(this) + elements.Sum(e => selection(e));
    }

    public double GetAverage(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selection)
    {
        return GetSum(elements, selection) / (elements.Count() + 1);
    }

    public static StatisticType? Create<StatisticType>(IEnumerable<StatisticElement> elements, CombineType combineType)
        where StatisticType : StatisticElement, new()
    {
        if (elements.Any() == false)
            return null;
        StatisticType element = new();
        element.Combine(elements, combineType);
        return element;

    }

    public int CompareTo(StatisticElement? other)
    {
        return Time.CompareTo(other?.Time);
    }
}

public enum CombineType
{
    Sum,
    Average
}
