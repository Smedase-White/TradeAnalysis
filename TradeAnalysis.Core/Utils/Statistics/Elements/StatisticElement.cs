namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement
{
    public DateTime Time { get; set; }

    public virtual void Combine(CombineType combineType, IEnumerable<StatisticElement> elements)
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

    public virtual void Combine(CombineType combineType, params StatisticElement[] elements)
        => Combine(combineType, elements as IEnumerable<StatisticElement>);

    public double GetSum(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selection)
    {
        return selection(this) + elements.Sum(e => selection(e));
    }

    public double GetAverage(IEnumerable<StatisticElement> elements, Func<StatisticElement, double> selection)
    {
        return GetSum(elements, selection) / (elements.Count() + 1);
    }
}

public enum CombineType
{
    Sum,
    Average
}
