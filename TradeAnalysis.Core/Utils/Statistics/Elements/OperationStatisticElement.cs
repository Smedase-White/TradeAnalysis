namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    public double Buy { get; set; } = 0;
    public double Sell { get; set; } = 0;

    public override bool IsEmpty
        => base.IsEmpty && (Buy == 0 && Sell == 0);

    public override void Combine(IEnumerable<StatisticElement> elements, CombineType combineType)
    {
        Combine((elements as IEnumerable<OperationStatisticElement>)!, combineType);
    }

    public virtual void Combine(IEnumerable<OperationStatisticElement> elements, CombineType combineType)
    {
        base.Combine(elements, combineType);
        int count = elements.Count() + 1;
        switch (combineType)
        {
            case CombineType.Sum:
                Buy += elements.Sum(e => e.Buy);
                Sell += elements.Sum(e => e.Sell);
                break;
            case CombineType.Average:
                Buy = (Buy + elements.Sum(e => e.Buy)) / count;
                Sell = (Sell + elements.Sum(e => e.Sell)) / count;
                break;
            default:
                break;
        }
    }
}
