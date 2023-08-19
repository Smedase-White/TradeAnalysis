using TradeAnalysis.Core.Utils.MarketItems;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.Core.Utils.Statistics;

public class AccountStatistics : Statistics<AccountStatisticElement>
{
    private readonly Account _account;

    public AccountStatistics(Account account)
        : base(Period.Hour)
    {
        _account = account;
        CalcStatistics();
    }

    public Account Account
    {
        get => _account;
    }

    public override void CalcStatistics()
    {
        if (_account.ItemsHistory is null || _account.ItemsHistory.Count == 0)
            return;

        MarketItem first = _account.ItemsHistory![0], last = _account.ItemsHistory![^1];
        DateTime startTime = Min(first.BuyInfo?.Time, first.SellInfo?.Time, _account.TransactionsHistory?[0].Info.Time),
            endTime = Max(last.BuyInfo?.Time, last.SellInfo?.Time, _account.TransactionsHistory?[^1].Info.Time);
        Data = new(from time in GetTimeCollection(startTime, endTime, Period.Hour)
                   select new AccountStatisticElement() { Time = time });

        FillStatisticValues(_account.ItemsHistory,
            item => item.BuyInfo is null ? NullTime : item.BuyInfo.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, value) => data.Buy = value);

        FillStatisticValues(_account.ItemsHistory,
            item => item.BuyInfo is null ? NullTime : item.BuyInfo.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, value) => data.BuyCount = value,
            values => values.Count());

        FillStatisticValues(_account.ItemsHistory,
            item => item.SellInfo is null ? NullTime : item.SellInfo.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, value) => data.Sell = value);

        FillStatisticValues(_account.ItemsHistory,
            item => item.SellInfo is null ? NullTime : item.SellInfo.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, value) => data.SellCount = value,
            values => values.Count());

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo is null ? NullTime : item.SellInfo.Time.ToInterval(),
            (item, _) => item.Profit!.Value,
            (data, value) => data.Profit = value);

        FillStatisticValues(_account.TradeHistory!,
            item => (item.BuyInfo!.Time, item.SellInfo!.Time),
            (item, _) => item.Profit!.Hourly,
            (data, value) => data.HourlyProfit = value);

        FillStatisticValues(_account.TransactionsHistory!,
            item => item.Info.Time.ToInterval(),
            (item, _) => item.Info.Amount,
            (data, value) => data.Transaction = value);

        FillStatisticValues(_account.DepositItemsHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, value) => data.DepositInItems = value);

        AccountStatisticElement prev = Data.First();
        foreach (AccountStatisticElement element in Data)
        {
            element.Cost = prev.Cost + element.Transaction + element.DepositInItems + element.Profit;
            prev = element;
        }
    }
}
