using TradeAnalysis.Core.MarketAPI;

namespace TradeAnalysis.Core.Utils.MarketItems;

public class MarketTransaction
{
    private readonly DealInfo _info;

    public MarketTransaction(DealInfo info)
    {
        _info = info;
    }

    public MarketTransaction(OperationHistoryPay pay)
        : this(new DealInfo(pay))
    { }

    public MarketTransaction(OperationHistoryTransfer transfer)
        : this(new DealInfo(transfer))
    { }

    public DealInfo Info
    {
        get => _info;
    }
}
