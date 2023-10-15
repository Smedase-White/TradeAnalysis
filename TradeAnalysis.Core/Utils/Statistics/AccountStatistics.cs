using TradeAnalysis.Core.Utils.Statistics.Elements;

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
        if (_account.StartHistoryTime.HasValue == false)
            return;

        CreateData(_account.StartHistoryTime!.Value, _account.EndHistoryTime!.Value);

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == false & item.BuyInfo is not null),
            item => item.BuyInfo!.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, values) => { data.Buy = values.Sum(); data.BuyCount = values.Count; });

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == false & item.SellInfo is not null),
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.SellInfo!.Amount,
            (data, values) => { data.Sell = values.Sum(); data.SellCount = values.Count; });

        FillStatisticValues(_account.FaultsHistory!,
            item => (item.BuyInfo is null ? item.SellInfo!.Time : item.BuyInfo!.Time).ToInterval(),
            (item, _) => 1,
            (data, values) => data.FaultsCount = values.Count);

        FillStatisticValues(_account.ItemsHistory!
            .Where(item => item.IsIgnored() == true & item.BuyInfo is not null),
            item => item.BuyInfo!.Time.ToInterval(),
            (item, _) => -item.BuyInfo!.Amount,
            (data, values) => data.BuyIgnoredCount = values.Count);

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
            (data, values) => data.AverageProfitPercent = values.Count > 0 ? values.Average() : 0);

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo!.Time.ToInterval(),
            (item, _) => item.Profit!.Duration,
            (data, values) => data.SellDuration = values.Count > 0 ? values.Average() : 0);

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
