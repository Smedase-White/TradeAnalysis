namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    [Combinable(CalculationType.Sum)]
    public double Profit { get; set; }

    [Combinable(CalculationType.Sum)]
    public double HourlyProfit { get; set; }

    [Combinable(CalculationType.Avg)]
    public double AverageProfitPercent { get; set; }

    [Combinable(CalculationType.Avg)]
    public double SellDuration { get; set; }

    public new TradeStatisticElement? Prev
    {
        get => base.Prev as TradeStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (HourlyProfit == 0);
}
