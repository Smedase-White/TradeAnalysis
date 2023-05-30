using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TradeOnAnalysis.Assets;
using TradeOnAnalysis.Assets.MarketAPI.Requests;
using TradeOnAnalysis.Assets.MarketAPI.Results;
using TradeOnAnalysis.Assets.MarketAPI.Utils;

namespace TradeOnAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int MaxItemHistory = 100;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Item.GetAllItems().Count == 0)
                await ParseItems();

            if (Item.GetAllItems().Count == 0)
                return;

            Task marketParseTask = ParseMarket();

            var items = from item in Item.GetAllItems()
                        where item.BuyInfo != null && item.SellInfo != null
                        orderby item.BuyInfo!.Date descending
                        select item;

            Grid.Display(items, item => (item.BuyInfo!.Date >= UserData.StartDate && item.SellInfo!.Date <= UserData.EndDate));

            await marketParseTask;

            ChartsPanel.DisplayAll(items, UserData.StartDate, UserData.EndDate);
        }

        private async Task ParseItems()
        {
            OperationHistoryRequest request = new(UserData.StartDate, UserData.EndDate, UserData.MarketApi);
            HttpStatusCode status = request.ResultMessage.StatusCode;

            StatusLabel.Content = $"{status}";
            if (status != HttpStatusCode.OK)
                return;

            OperationHistoryResult result = request.Result!;
            foreach (OperationHistoryElement element in result.History)
                Item.LoadFromAPI(element);
        }

        private async Task ParseMarket()
        {
            if (Item.GetAllItems().Count == 0)
                return;

            List<Task> tasks = new();
            int count = 0;
            foreach (Item item in Item.GetAllItems())
            {
                tasks.Add(item.LoadHistory(UserData.MarketApi));
                if (++count >= MaxItemHistory)
                    break;
            }
            await Task.WhenAll(tasks);
        }
    }
}
