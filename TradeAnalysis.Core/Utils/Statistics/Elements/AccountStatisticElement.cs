namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class AccountStatisticElement : TradeStatisticElement
{
    private double _cost = 0;

    public double Cost
    {
        get => _cost;
        set => _cost = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Cost == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Cost = (elements.Last() as AccountStatisticElement)!.Cost;
    }
}
