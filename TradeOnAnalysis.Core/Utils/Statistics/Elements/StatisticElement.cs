namespace TradeOnAnalysis.Core.Utils.Statistics.Elements;

public class StatisticElement
{
    public DateTime Date { get; set; }

    public virtual void Combine(CombineType combineType, IEnumerable<StatisticElement> elements)
    {
        switch (combineType)
        {
            case CombineType.Sum:
                DateTime maxDate = elements.OrderBy(e => e.Date).Last().Date;
                if (maxDate > Date)
                    Date = maxDate;
                break;
            case CombineType.Average:
                Date.AddDays(elements.Sum(e => (e.Date - Date).Days) / (elements.Count() + 1));
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
