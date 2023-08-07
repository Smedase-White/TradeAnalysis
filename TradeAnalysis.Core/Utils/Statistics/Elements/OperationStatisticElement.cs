namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    private double _buy = 0;
    private double _sell = 0;

    public double Buy 
    {
        get => _buy;
        set => _buy = value;
    }
    public double Sell 
    {
        get => _sell;
        set => _sell = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Buy == 0 && Sell == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Sum(ref _buy, elements, e => (e as OperationStatisticElement)!.Buy);
        Sum(ref _sell, elements, e => (e as OperationStatisticElement)!.Sell);
    }
}
