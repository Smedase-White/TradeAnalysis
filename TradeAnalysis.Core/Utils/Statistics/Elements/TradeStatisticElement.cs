﻿namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : OperationStatisticElement
{
    private double? _profit;
    private double? _hourlyProfit;

    public double Profit
    {
        get => _profit ?? 0;
        set => _profit = value;
    }
    public double HourlyProfit
    {
        get => _hourlyProfit ?? 0;
        set => _hourlyProfit = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (HourlyProfit == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Sum(ref _profit, elements, e => (e as TradeStatisticElement)!._profit);
        Sum(ref _hourlyProfit, elements, e => (e as TradeStatisticElement)!._hourlyProfit);
    }
}
