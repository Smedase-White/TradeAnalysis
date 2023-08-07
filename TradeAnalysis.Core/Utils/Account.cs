using System.Collections.Immutable;
using System.Net;

using TradeAnalysis.Core.MarketAPI;
using TradeAnalysis.Core.Utils.Statistics;

namespace TradeAnalysis.Core.Utils
{
    public class Account
    {
        private readonly DateTime StartTime = new(2001, 1, 1, 0, 0, 0);
        public const int MaxItemHistory = 10;

        public string MarketApi { get; init; }

        public IImmutableList<Item>? History { get; private set; }
        public IImmutableList<Item>? TradeHistory { get; private set; }
        public TradeStatistics? TradeStatistics { get; private set; }

        public Account(string marketApi)
        {
            MarketApi = marketApi;
        }

        public HttpStatusCode LoadHistory()
        {
            OperationHistoryRequest request = new(StartTime, DateTime.Now, MarketApi);
            HttpStatusCode status = request.ResultMessage.StatusCode;

            if (status != HttpStatusCode.OK)
                return status;

            OperationHistoryResult result = request.Result!;

            List<Item> history = new();
            foreach (OperationHistoryElement element in result.History)
            {
                if (element.Stage == TradeStage.TimedOut)
                    continue;
                if (element.Event == EventType.Transaction)
                    continue;
                history.Add(Item.LoadFromAPI(element));
            }
            history.Reverse();

            History = history.ToImmutableList();
            TradeHistory = GetTradeHistory(history).ToImmutableList();

            return status;
        }

        public void CalcStatistics()
        {
            TradeStatistics = new(this);
        }

        private static List<Item> GetTradeHistory(List<Item> history)
        {
            List<Item> tradeHistory = new(history);

            for (int i = 0; i < tradeHistory.Count; i++)
            {
                if (tradeHistory[i].Name.Contains("Sealed Graffiti"))
                {
                    tradeHistory.RemoveAt(i);
                    i--;
                    continue;
                }

                if (tradeHistory[i].BuyInfo is null)
                {
                    tradeHistory.RemoveAt(i);
                    i--;
                    continue;
                }

                for (int j = i + 1; j < tradeHistory.Count; j++)
                {
                    if (tradeHistory[i].Name != tradeHistory[j].Name)
                        continue;

                    if (tradeHistory[j].SellInfo is null)
                        continue;

                    tradeHistory[i] = new(tradeHistory[i]);
                    tradeHistory[i].SellInfo = tradeHistory[j].SellInfo;
                    tradeHistory[i].Profit = new(tradeHistory[i].BuyInfo!, tradeHistory[i].SellInfo!);
                    tradeHistory.RemoveAt(j);
                    break;
                }

                if (tradeHistory[i].SellInfo is null)
                {
                    tradeHistory.RemoveAt(i);
                    i--;
                    continue;
                }
            }

            return tradeHistory;
        }
    }
}
