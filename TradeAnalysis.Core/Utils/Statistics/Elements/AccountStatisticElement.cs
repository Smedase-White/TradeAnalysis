namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class AccountStatisticElement : TradeStatisticElement
{
    private double? _cost;

    public double Cost
    {
        get => _cost ?? 0;
        set => _cost = value;
    }

    public new AccountStatisticElement? Prev
    {
        get => base.Prev as AccountStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty;

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        _cost = (elements.Last() as AccountStatisticElement)!._cost;
    }
}
