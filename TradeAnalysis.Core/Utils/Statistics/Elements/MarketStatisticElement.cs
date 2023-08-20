namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class MarketStatisticElement : StatisticElement
{
    private double _price = 1;
    private double _count = 0;

    public double Price
    {
        get => _price;
        set => _price = value;
    }

    public double Count
    {
        get => _count;
        set => _count = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Price == 1 && Count == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Average(ref _price, elements, e => (e as MarketStatisticElement)!.Price);
        Sum(ref _count, elements, e => (e as MarketStatisticElement)!.Count);
    }
}
