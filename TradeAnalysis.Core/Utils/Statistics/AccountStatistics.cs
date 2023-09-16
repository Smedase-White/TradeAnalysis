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
        Data = new(GetTimeCollection(startTime, endTime, Period.Hour)
            .Select(time => new AccountStatisticElement() { Time = time }));

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == false & item.BuyInfo is not null),
            item => item.BuyInfo!.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, values) => { data.Buy = values.Sum(); data.BuyCount = values.Count(); });

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == false & item.SellInfo is not null),
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, values) => { data.Sell = values.Sum(); data.SellCount = values.Count(); });

        FillStatisticValues(_account.FaultsHistory!,
            item => (item.BuyInfo is null ? item.SellInfo!.Time : item.BuyInfo!.Time).ToInterval(),
            (item, _) => 1,
            (data, values) => data.FaultsCount = values.Count());

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == true & item.BuyInfo is not null),
            item => item.BuyInfo!.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, values) => data.BuyIgnoredCount = values.Count());

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.Profit!.Value,
            (data, values) => data.Profit = values.Sum());

        FillStatisticValues(_account.TradeHistory!,
            item => (item.BuyInfo!.Time, item.SellInfo!.Time),
            (item, _) => item.Profit!.Hourly,
            (data, values) => data.HourlyProfit = values.Sum());

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.Profit!.Percent,
            (data, values) => data.AverageProfitPercent = values.Average());

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.Profit!.Duration,
            (data, values) => data.SellDuration = values.Average());

        FillStatisticValues(_account.TransactionsHistory!,
            item => item.Info.Time.ToInterval(),
            (item, _) => item.Info.Amount,
            (data, values) => data.Transaction = values.Sum());

        FillStatisticValues(_account.DepositItemsHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, values) => data.DepositInItems = values.Sum());

        AccountStatisticElement prev = Data.First();
        foreach (AccountStatisticElement element in Data)
        {
            element.Cost = prev.Cost + element.Transaction + element.DepositInItems + element.Profit;
            prev = element;
        }
    }
}
