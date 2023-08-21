namespace TradeAnalysis.Core.Utils.Statistics.Elements;

public class OperationStatisticElement : StatisticElement
{
    private double? _buy;
    private double? _buyCount;
    private double? _sell;
    private double? _sellCount;
    private double? _transaction;
    private double? _depositInItems;

    public double Buy
    {
        get => _buy ?? 0;
        set => _buy = value;
    }

    public double BuyCount
    {
        get => _buyCount ?? 0;
        set => _buyCount = value;
    }

    public double Sell
    {
        get => _sell ?? 0;
        set => _sell = value;
    }

    public double SellCount
    {
        get => _sellCount ?? 0;
        set => _sellCount = value;
    }

    public double Transaction
    {
        get => _transaction ?? 0;
        set => _transaction = value;
    }

    public double DepositInItems
    {
        get => _depositInItems ?? 0;
        set => _depositInItems = value;
    }

    public override bool IsEmpty
        => base.IsEmpty && (BuyCount == 0 && SellCount == 0 && Transaction == 0);

    public override void Combine<StatisticType>(IEnumerable<StatisticType> elements)
    {
        base.Combine(elements);
        Sum(ref _buy, elements, e => (e as OperationStatisticElement)!._buy);
        Sum(ref _buyCount, elements, e => (e as OperationStatisticElement)!._buyCount);
        Sum(ref _sell, elements, e => (e as OperationStatisticElement)!._sell);
        Sum(ref _sellCount, elements, e => (e as OperationStatisticElement)!._sellCount);
        Sum(ref _transaction, elements, e => (e as OperationStatisticElement)!._transaction);
        Sum(ref _depositInItems, elements, e => (e as OperationStatisticElement)!._depositInItems);
    }
}
