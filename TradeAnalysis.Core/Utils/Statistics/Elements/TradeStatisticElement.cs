namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    public double Profit { get; set; } = 0;
    public double HourlyProfit { get; set; } = 0;

    public override void Combine(CombineType combineType, IEnumerable<StatisticElement> elements)
    {
        Combine(combineType, (elements as IEnumerable<TradeStatisticElement>)!);
    }

    public virtual void Combine(CombineType combineType, IEnumerable<TradeStatisticElement> elements)
    {
        base.Combine(combineType, elements);
        int count = elements.Count() + 1;
        switch (combineType)
        {
            case CombineType.Sum:
                Profit += elements.Sum(e => e.Profit);
                HourlyProfit += elements.Sum(e => e.HourlyProfit);
                break;
            case CombineType.Average:
                Profit = (Profit + elements.Sum(e => e.Profit)) / count;
                HourlyProfit = (HourlyProfit + elements.Sum(e => e.HourlyProfit)) / count;
                break;
            default:
                break;
        }
    }

    public virtual void Combine(CombineType combineType, params TradeStatisticElement[] elements)
        => Combine(combineType, elements as IEnumerable<TradeStatisticElement>);
}
