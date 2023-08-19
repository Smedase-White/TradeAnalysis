namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    private double _buy = 0;
    private double _buyCount = 0;
    private double _sell = 0;
    private double _sellCount = 0;
    private double _transaction = 0;
    private double _depositInItems = 0;

    public double Buy
    {
        get => _buy;
        set => _buy = value;
    }

    public double BuyCount
    {
        get => _buyCount; 
        set => _buyCount = value;
    }

    public double Sell
    {
        get => _sell;
        set => _sell = value;
    }

    public double SellCount
    {
        get => _sellCount;
        set => _sellCount = value;
    }

    public double Transaction
    {
        get => _transaction;
        set => _transaction = value;
    }

    public double DepositInItems
    {
        get => _depositInItems;
        set => _depositInItems = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (Buy == 0 && Sell == 0 && Transaction == 0 && DepositInItems == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Sum(ref _buy, elements, e => (e as OperationStatisticElement)!.Buy);
        Sum(ref _buyCount, elements, e => (e as OperationStatisticElement)!.BuyCount);
        Sum(ref _sell, elements, e => (e as OperationStatisticElement)!.Sell);
        Sum(ref _sellCount, elements, e => (e as OperationStatisticElement)!.SellCount);
        Sum(ref _transaction, elements, e => (e as OperationStatisticElement)!.Transaction);
        Sum(ref _depositInItems, elements, e => (e as OperationStatisticElement)!.DepositInItems);
    }
}
