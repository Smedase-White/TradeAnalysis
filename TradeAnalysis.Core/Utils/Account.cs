using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;
using TradeAnalysis.Core.Utils.MarketItems;
using TradeAnalysis.Core.Utils.Statistics;

namespace TradeAnalysis.Core.Utils
{
    public class Account
    {
        private static readonly DateTime StartTime = new(2021, 1, 1, 0, 0, 0);
        private const int TradeDaysLimit = 100;
        private static readonly string[] IgnoredItems = { "Sealed Graffiti" };

        private readonly string _marketApi;

        private IImmutableList<MarketItem>? _itemsHistory;
        private IImmutableList<MarketTransaction>? _transactionsHistory;

        private IImmutableList<MarketItem>? _tradeHistory;
        private IImmutableList<MarketItem>? _depositItemsHistory;

        private AccountStatistics? _statistics;

        public Account(string marketApi)
        {
            _marketApi = marketApi;
        }

        public string MarketApi
        {
            get => _marketApi;
        }

        public IImmutableList<MarketItem>? ItemsHistory
        {
            get => _itemsHistory;
            private set
            {
                _itemsHistory = value;
                if (_itemsHistory is null)
                    return;
                AnalisysItemsHistory(_itemsHistory);
            }
        }

        public IImmutableList<MarketTransaction>? TransactionsHistory
        {
            get => _transactionsHistory;
            private set => _transactionsHistory = value;
        }

        public IImmutableList<MarketItem>? TradeHistory
        {
            get => _tradeHistory;
            private set => _tradeHistory = value;
        }

        public IImmutableList<MarketItem>? DepositItemsHistory
        {
            get => _depositItemsHistory;
            private set => _depositItemsHistory = value;
        }

        public AccountStatistics? Statistics
        {
            get => _statistics;
            private set => _statistics = value;
        }

        public HttpStatusCode LoadHistory()
        {
            OperationHistoryRequest request = new(StartTime, DateTime.Now, MarketApi);
            HttpStatusCode status = request.ResultMessage.StatusCode;
            if (status != HttpStatusCode.OK)
                return status;

            List<OperationHistoryBase> results = request.Result!.History;
            results.Reverse();

            List<MarketItem> itemHistory = new();
            List<MarketTransaction> transactionHistory = new();
            foreach (OperationHistoryBase element in results)
            {
                switch (element)
                {
                    case OperationHistoryItem item: 
                        if (item.Stage == Stage.TimedOut)
                            continue;
                        itemHistory.Add(new(item)); 
                        break;
                    case OperationHistoryPay pay:
                        transactionHistory.Add(new(pay));
                        break;
                    case OperationHistoryTransfer transfer:
                        transactionHistory.Add(new(transfer));
                        break;
                }
            }

            foreach (string ignoredItem in IgnoredItems)
                itemHistory = itemHistory.Where(item => IsIgnored(item.Name) == false).ToList();

            ItemsHistory = itemHistory.ToImmutableList();
            TransactionsHistory = transactionHistory.ToImmutableList();

            Statistics = new(this);

            return status;
        }

        private static bool IsIgnored(string name)
        {
            foreach (string ignoredItem in IgnoredItems)
            {
                if (name.Contains(ignoredItem))
                    return true;
            }
            return false;
        }

        private void AnalisysItemsHistory(IImmutableList<MarketItem> history)
        {
            List<MarketItem> trades = new(history);
            List<MarketItem> depositItems = new();

            for (int i = 0; i < trades.Count; i++)
            {
                if (trades[i].BuyInfo is null)
                {
                    depositItems.Add(trades[i]);
                    trades.RemoveAt(i);
                    i--;
                    continue;
                }

                for (int j = i + 1; j < trades.Count; j++)
                {
                    if (trades[i].Name != trades[j].Name)
                        continue;

                    if (trades[j].SellInfo is null)
                        continue;

                    if ((trades[j].SellInfo!.Time - trades[i].BuyInfo!.Time).TotalDays > TradeDaysLimit)
                        continue;

                    trades[i] = new(trades[i]);
                    trades[i].SellInfo = trades[j].SellInfo;
                    trades.RemoveAt(j);
                    break;
                }

                if (trades[i].SellInfo is null)
                {
                    trades.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            TradeHistory = trades.ToImmutableList();
            DepositItemsHistory = depositItems.ToImmutableList();
        }
    }
}
