using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TradeOnAnalysis.Core.MarketAPI;

namespace TradeOnAnalysis.Core.Utils
{
    public class AccountStats
    {
        public IImmutableList<Item>? History { get; private set; }
        public IImmutableList<Item>? TradeHistory { get; private set; }

        public string MarketApi { get; init; }

        public AccountStats(string marketApi)
        {
            MarketApi = marketApi;
            LoadHistory();
        }

        private readonly DateTime StartDate = new(2001, 1, 1);
        public HttpStatusCode LoadHistory()
        {
            OperationHistoryRequest request = new(StartDate, DateTime.Today, MarketApi);
            HttpStatusCode status = request.ResultMessage.StatusCode;

            if (status != HttpStatusCode.OK)
                return status;

            OperationHistoryResult result = request.Result!;

            List<Item> history = new();
            foreach (OperationHistoryElement element in result.History)
            {
                if (element.TradeStage == TradeStage.TimedOut)
                    continue;
                history.Add(Item.LoadFromAPI(element));
            }
            history.Reverse();

            History = history.ToImmutableList();
            TradeHistory = GetTradeHistory(history).ToImmutableList();

            return status;
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

                    tradeHistory[i].SellInfo = tradeHistory[j].SellInfo;
                    tradeHistory.RemoveAt(j);
                    break;
                }
            }

            return tradeHistory;
        }
    }
}
