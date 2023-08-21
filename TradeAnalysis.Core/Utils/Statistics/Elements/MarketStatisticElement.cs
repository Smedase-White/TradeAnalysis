namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class MarketStatisticElement : StatisticElement
{
    private double? _price;
    private double? _count;

    public double Price
    {
        get => _price ?? 1;
        set => _price = value;
    }

    public double Count
    {
        get => _count ?? 0;
        set => _count = value;
    }
    public new MarketStatisticElement? Prev
    {
        get => base.Prev as MarketStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Count == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Average(ref _price, elements, e => (e as MarketStatisticElement)!._price);
        Sum(ref _count, elements, e => (e as MarketStatisticElement)!._count);
    }
}
