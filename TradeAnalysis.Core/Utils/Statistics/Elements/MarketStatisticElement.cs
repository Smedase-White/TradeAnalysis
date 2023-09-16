namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class MarketStatisticElement : StatisticElement
{
    [Combinable(CalculationType.Avg)]
    public double Price { get; set; } = 1;

    [Combinable(CalculationType.Sum)]
    public double Count { get; set; } = 0;

    public new MarketStatisticElement? Prev
    {
        get => base.Prev as MarketStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Count == 0);
}
