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
        private const int TradelessDaysLimit = 100;
        private static readonly string[] IgnoredItems = { "Sealed Graffiti" };
        private const int MaxParseCount = 500;
        private const int MaxParallelRequestCount = 5;

        private readonly string _marketApi;

        private ImmutableList<MarketItem>? _itemsHistory;
        private ImmutableList<MarketItem>? _faultsHistory;
        private ImmutableList<MarketTransaction>? _transactionsHistory;

        private ImmutableList<MarketItem>? _tradeHistory;
        private ImmutableList<MarketItem>? _depositItemsHistory;

        private ImmutableList<MarketItem>? _parsedItems;

        private AccountStatistics? _statistics;
        private MarketStatistics? _marketStatistics;

        public Account(string marketApi)
        {
            _marketApi = marketApi;
        }

        public string MarketApi
        {
            get => _marketApi;
        }

        public ImmutableList<MarketItem>? ItemsHistory
        {
            get => _itemsHistory;
            private set
            {
                _itemsHistory = value;
                if (_itemsHistory is null)
                    return;
                AnalisysItemsHistory();
            }
        }

        public IEnumerable<MarketItem>? IgnoredItemsHistory
        {
            get => _itemsHistory?.Where(item => IsIgnored(item.Name) == true);
        }

        public IEnumerable<MarketItem>? NotIgnoredItemsHistory
        {
            get => _itemsHistory?.Where(item => IsIgnored(item.Name) == false);
        }

        public ImmutableList<MarketItem>? FaultsHistory 
        { 
            get => _faultsHistory; 
            private set => _faultsHistory = value; 
        }

        public ImmutableList<MarketTransaction>? TransactionsHistory
        {
            get => _transactionsHistory;
            private set => _transactionsHistory = value;
        }

        public ImmutableList<MarketItem>? TradeHistory
        {
            get => _tradeHistory;
            private set => _tradeHistory = value;
        }

        public ImmutableList<MarketItem>? DepositItemsHistory
        {
            get => _depositItemsHistory;
            private set => _depositItemsHistory = value;
        }

        public ImmutableList<MarketItem>? ParsedItems
        {
            get => _parsedItems;
            private set => _parsedItems = value;
        }

        public AccountStatistics? Statistics
        {
            get => _statistics;
            private set => _statistics = value;
        }

        public MarketStatistics? MarketStatistics
        {
            get => _marketStatistics;
            private set => _marketStatistics = value;
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
            List<MarketItem> faultsHistory = new();
            List<MarketTransaction> transactionHistory = new();
            foreach (OperationHistoryBase element in results)
            {
                switch (element)
                {
                    case OperationHistoryItem item:
                        switch (item.Stage)
                        {
                            case Stage.New:
                                break;
                            case Stage.Given:
                                itemHistory.Add(new(item));
                                break;
                            case Stage.TimedOut:
                                faultsHistory.Add(new(item));
                                break;
                        }
                        break;
                    case OperationHistoryPay pay:
                        transactionHistory.Add(new(pay));
                        break;
                    case OperationHistoryTransfer transfer:
                        transactionHistory.Add(new(transfer));
                        break;
                }
            }

            ItemsHistory = itemHistory.ToImmutableList();
            TransactionsHistory = transactionHistory.ToImmutableList();

            Statistics = new(this);

            return status;
        }

        private void AnalisysItemsHistory()
        {
            List<MarketItem> trades = new(NotIgnoredItemsHistory!);
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

                    if ((trades[j].SellInfo!.Time - trades[i].BuyInfo!.Time).TotalDays > TradelessDaysLimit)
                        continue;

                    trades[i] = new(trades[i]) { SellInfo = trades[j].SellInfo };
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

        public void ParseItems()
        {
            if (ItemsHistory is null || ItemsHistory.Count == 0)
                return;

            List<MarketItem> parsedItems = new();
            if (ItemsHistory.Count < MaxParseCount)
            {
                parsedItems.AddRange(ItemsHistory);
            }
            else
            {
                Random rand = new();
                for (int i = 0; i < MaxParseCount; i++)
                    parsedItems.Add(ItemsHistory![rand.Next(ItemsHistory.Count)]);
            }

            List<Action> parseActions = new();
            foreach (MarketItem item in parsedItems)
                parseActions.Add(() => item.LoadHistory(MarketApi));
            ParallelOptions options = new() { MaxDegreeOfParallelism = MaxParallelRequestCount };
            Parallel.Invoke(options, parseActions.ToArray());

            ParsedItems = parsedItems.ToImmutableList();

            MarketStatistics = new(this);
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
    }
}
