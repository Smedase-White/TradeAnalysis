namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    [Combinable(CalculationType.Sum)]
    public double Buy { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double BuyCount { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double Sell { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double SellCount { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double Transaction { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double DepositInItems { get; set; } = 0;

    public new OperationStatisticElement? Prev
    {
        get => base.Prev as OperationStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (BuyCount == 0 && SellCount == 0 && Transaction == 0);
}
