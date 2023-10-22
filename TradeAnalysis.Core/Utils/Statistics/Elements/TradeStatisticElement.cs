namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    [Combinable(CalculationType.Sum)]
    public double Profit { get; set; } = 0;

    [Combinable(CalculationType.Sum)]
    public double HourlyProfit { get; set; } = 0;

    [Combinable(CalculationType.Avg)]
    public double AverageProfitPercent { get; set; } = 0;

    [Combinable(CalculationType.Avg)]
    public double SellDuration { get; set; } = 0;

    public new TradeStatisticElement? Prev
    {
        get => base.Prev as TradeStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (HourlyProfit == 0);
}
