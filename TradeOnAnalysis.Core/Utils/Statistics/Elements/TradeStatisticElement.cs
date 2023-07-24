namespace TradeOnAnalysis.Core.Utils.Statistics.Elements;

public class TradeStatisticElement : StatisticElement
{
    public double Buy { get; set; } = 0;
    public double Sell { get; set; } = 0;
    public double Profit { get; set; } = 0;
    public double DailyProfit { get; set; } = 0;

    public double MarketAnalisys { get; set; } = 1;

    public override void Combine(CombineType combineType, IEnumerable<StatisticElement> elements)
    {
        Combine(combineType, (elements as IEnumerable<TradeStatisticElement>)!);
    }

    public virtual void Combine(CombineType combineType, IEnumerable<TradeStatisticElement> elements)
    {
        base.Combine(combineType, elements);
        int count = elements.Count() + 1;
        switch (combineType)
        {
            case CombineType.Sum:
                Buy += elements.Sum(e => e.Buy);
                Sell += elements.Sum(e => e.Sell);
                Profit += elements.Sum(e => e.Profit);
                DailyProfit += elements.Sum(e => e.DailyProfit);
                MarketAnalisys = (MarketAnalisys + elements.Sum(e => e.MarketAnalisys)) / count;
                break;
            case CombineType.Average:
                Buy = (Buy + elements.Sum(e => e.Buy)) / count;
                Sell = (Sell + elements.Sum(e => e.Sell)) / count;
                Profit = (Profit + elements.Sum(e => e.Profit)) / count;
                DailyProfit = (DailyProfit + elements.Sum(e => e.DailyProfit)) / count;
                MarketAnalisys = (MarketAnalisys + elements.Sum(e => e.MarketAnalisys)) / count;
                break;
            default:
                break;
        }
    }

    public virtual void Combine(CombineType combineType, params TradeStatisticElement[] elements)
        => Combine(combineType, elements as IEnumerable<TradeStatisticElement>);
}
