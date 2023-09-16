namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class AccountStatisticElement : TradeStatisticElement
{
    [Combinable(CalculationType.Last)]
    public double Cost { get; set; } = 0;

    public new AccountStatisticElement? Prev
    {
        get => base.Prev as AccountStatisticElement;
        set => base.Prev = value;
    }

    public override bool IsEmpty
        => base.IsEmpty;
}
