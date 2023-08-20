using TradeAnalysis.Core.MarketAPI;

namespace TradeAnalysis.Core.Utils.MarketItems;

public class DealInfo
{
    private readonly DateTime _time;
    private readonly double _amount;

    public DealInfo(DateTime time, double amount)
    {
        _time = time;
        _amount = amount;
    }

    public DealInfo(OperationHistoryItem item)
    {
        _time = item.Time!.Value;
        _amount = item.Event switch
        {
            EventType.Sell => item.Recieved * item.SellerRate,
            EventType.Buy => -item.Paid * item.BuyerRate,
            _ => 0
        } ?? 0;
    }

    public DealInfo(OperationHistoryPay pay)
    {
        _time = pay.Time!.Value;
        _amount = pay.Event switch
        {
            EventType.PayIn => pay.AmountIn * pay.Rate,
            EventType.PayOut => -pay.AmountOut * pay.Rate,
            _ => 0
        } ?? 0;
    }

    public DealInfo(OperationHistoryTransfer transfer)
    {
        _time = transfer.Time!.Value;
        _amount = transfer.Event switch
        {
            EventType.TransferIn => transfer.AmountTo * transfer.RateTo,
            EventType.TransferOut => -transfer.AmountFrom * transfer.RateFrom,
            _ => 0
        } ?? 0;
    }

    public DateTime Time
    {
        get => _time;
    }

    public double Amount
    {
        get => _amount;
    }
}