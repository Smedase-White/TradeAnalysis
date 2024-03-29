﻿using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;
using TradeAnalysis.Core.Utils.MarketItems;
using TradeAnalysis.Core.Utils.Statistics;

namespace TradeAnalysis.Core.Utils
{
    public class Account
    {
        private static readonly DateTime StartTime = new(2001, 1, 1, 0, 0, 0);
        private const int TradelessDaysLimit = 100;
        private const int MaxParseCount = 500;
        private const int MaxParallelRequestCount = 5;

        private readonly string _marketApis;

        private DateTime? _startHistoryTime, _endHistoryTime;

        private ImmutableList<MarketItem>? _itemsHistory;
        private ImmutableList<MarketItem>? _faultsHistory;
        private ImmutableList<MarketTransaction>? _transactionsHistory;

        private ImmutableList<MarketItem>? _tradeHistory;
        private ImmutableList<MarketItem>? _depositItemsHistory;
        private ImmutableList<MarketItem>? _unsoldItemsHistory;

        private ImmutableList<MarketItem>? _parsedItems;

        private AccountStatistics? _statistics;
        private MarketStatistics? _marketStatistics;

        public Account(string marketApis)
        {
            _marketApis = marketApis;
        }

        public string MarketApis
        {
            get => _marketApis;
        }

        public DateTime? StartHistoryTime
        {
            get => _startHistoryTime;
        }

        public DateTime? EndHistoryTime
        {
            get => _endHistoryTime;
        }

        public ImmutableList<MarketItem>? ItemsHistory
        {
            get => _itemsHistory;
            private set
            {
                _itemsHistory = value;
                if (_itemsHistory is null)
                    return;
                AnalysisItemsHistory();
            }
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

        public ImmutableList<MarketItem>? UnsoldItemsHistory
        {
            get => _unsoldItemsHistory;
            private set => _unsoldItemsHistory = value;
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
            List<OperationHistoryBase> results = new();
            HttpStatusCode status = HttpStatusCode.NotFound;
            foreach (string marketApi in MarketApis.Split("\r\n"))
            {
                OperationHistoryRequest request = new(StartTime, DateTime.Now, marketApi);
                status = request.ResultMessage.StatusCode;
                if (status != HttpStatusCode.OK)
                    return status;
                results.AddRange(request.Result!.History);
            }
            if (results.Count == 0)
                return status;

            results = results.OrderBy(item => item.Time).ToList();

            _startHistoryTime = results.First().Time;
            _endHistoryTime = results.Last().Time;

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
            FaultsHistory = faultsHistory.ToImmutableList();
            TransactionsHistory = transactionHistory.ToImmutableList();

            Statistics = new(this);

            return HttpStatusCode.OK;
        }

        private void AnalysisItemsHistory()
        {
            List<MarketItem> trades = new(_itemsHistory!.Where(item => item.IsIgnored() == false));
            List<MarketItem> depositItems = new();
            List<MarketItem> unsoldItems = new();

            int i = 0;
            while (i < trades.Count)
            {
                if (trades[i].BuyInfo is null)
                {
                    depositItems.Add(trades[i]);
                    trades.RemoveAt(i);
                    continue;
                }

                for (int j = i + 1; j < trades.Count; j++)
                {
                    if (trades[i].Name != trades[j].Name)
                        continue;

                    if (trades[j].SellInfo is null)
                        continue;

                    if ((trades[j].SellInfo!.Time - trades[i].BuyInfo!.Time).TotalDays > TradelessDaysLimit)
                        break;

                    trades[i] = new(trades[i]) { SellInfo = trades[j].SellInfo };
                    trades.RemoveAt(j);
                    break;
                }

                if (trades[i].SellInfo is null)
                {
                    unsoldItems.Add(trades[i]);
                    trades.RemoveAt(i);
                    continue;
                }

                i++;
            }

            TradeHistory = trades.ToImmutableList();
            DepositItemsHistory = depositItems.ToImmutableList();
            UnsoldItemsHistory = unsoldItems.ToImmutableList();
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
                parseActions.Add(() => item.LoadHistory(MarketApis));
            ParallelOptions options = new() { MaxDegreeOfParallelism = MaxParallelRequestCount };
            Parallel.Invoke(options, parseActions.ToArray());

            ParsedItems = parsedItems.ToImmutableList();

            MarketStatistics = new(this);
        }
    }
}
