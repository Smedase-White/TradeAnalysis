using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Item;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.Core.Utils.Statistics;

public class TradeStatistics : Statistics<TradeStatisticElement>
{
    private readonly Account _account;

    public TradeStatistics(Account account)
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
        if (_account.History is null || _account.History.Count == 0)
            return;

        MarketItem first = _account.History![0], last = _account.History![^1];
        DateTime startTime = Min(first.BuyInfo?.Time, first.SellInfo?.Time),
            endTime = Max(last.BuyInfo?.Time, last.SellInfo?.Time);
        Data = new(from time in GetTimeCollection(startTime, endTime, Period.Hour)
                   select new TradeStatisticElement() { Time = time });

        FillStatisticValues(_account.History,
            item => item.BuyInfo is null ? NullTime : item.BuyInfo.Time.ToInterval(),
            (item, _) => item.BuyInfo!.Price,
            (data, value) => data.Buy = value);

        FillStatisticValues(_account.History,
            item => item.SellInfo is null ? NullTime : item.SellInfo.Time.ToInterval(),
            (item, _) => item.SellInfo!.Price,
            (data, value) => data.Sell = value);

        FillStatisticValues(_account.TradeHistory!,
            item => item.SellInfo is null ? NullTime : item.SellInfo.Time.ToInterval(),
            (item, _) => item.Profit!.Value,
            (data, value) => data.Profit = value);

        FillStatisticValues(_account.TradeHistory!,
            item => (item.BuyInfo!.Time, item.SellInfo!.Time),
            (item, _) => item.Profit!.Hourly,
            (data, value) => data.HourlyProfit = value);
    }
}
