namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    public double Profit { get; set; } = 0;
    public double HourlyProfit { get; set; } = 0;

    public override bool IsEmpty
        => base.IsEmpty && (Profit == 0 && HourlyProfit == 0);

    public override void Combine(IEnumerable<StatisticElement> elements, CombineType combineType)
    {
        Combine((elements as IEnumerable<TradeStatisticElement>)!, combineType);
    }

    public virtual void Combine(IEnumerable<TradeStatisticElement> elements, CombineType combineType)
    {
        base.Combine(elements, combineType);
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
}
