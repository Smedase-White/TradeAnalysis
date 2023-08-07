namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    private double _profit = 0;
    private double _hourlyProfit = 0;

    public double Profit 
    { 
        get => _profit; 
        set => _profit = value; 
    }
    public double HourlyProfit 
    { 
        get => _hourlyProfit; 
        set => _hourlyProfit = value; 
    }

    public override bool IsEmpty
        => base.IsEmpty && (Profit == 0 && HourlyProfit == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Sum(ref _profit, elements, e => (e as TradeStatisticElement)!.Profit);
        Sum(ref _hourlyProfit, elements, e => (e as TradeStatisticElement)!.HourlyProfit);
    }
}
