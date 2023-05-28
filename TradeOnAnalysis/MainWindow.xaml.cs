using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using TradeOnAnalysis.Assets;
using TradeOnAnalysis.Assets.MarketAPI;
using TradeOnAnalysis.Assets.MarketAPI.Answer;

namespace TradeOnAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://market.csgo.com/api/"),
        };

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Item.GetAllItems().Count == 0)
                ParseItems();

            if (Item.GetAllItems().Count == 0)
                return;

            var items = from item in Item.GetAllItems()
                        where item.BuyInfo != null && item.SellInfo != null
                        orderby item.BuyInfo!.Date descending
                        select item;

            Grid.Display(items, item => (item.BuyInfo!.Date >= UserData.StartDate && item.SellInfo!.Date <= UserData.EndDate));

            BuySellChart.Clear();
            ProfitChart.Clear();
            DailyProfitChart.Clear();

            BuySellChart.DisplayBuys(Item.GetAllItems().Where(x => x.BuyInfo != null), UserData.StartDate, UserData.EndDate);
            BuySellChart.DisplaySells(Item.GetAllItems().Where(x => x.SellInfo != null), UserData.StartDate, UserData.EndDate);
            ProfitChart.DisplayProfit(items, UserData.StartDate, UserData.EndDate);
            DailyProfitChart.DisplayDailyProfit(items, UserData.StartDate, UserData.EndDate);
        }

        private static long DateTimeToUnix(DateTime dateTime)
            => ((DateTimeOffset)dateTime).ToUnixTimeSeconds();

        private void ParseItems()
        {
            var task = httpClient.GetAsync($"OperationHistory/" +
                $"{DateTimeToUnix(UserData.StartDate)}/" +
                $"{DateTimeToUnix(UserData.EndDate)}/" +
                $"?key={UserData.MarketApi}");

            var result = task.Result;
            StatusLabel.Content = $"{result.StatusCode}";
            if (result.StatusCode != HttpStatusCode.OK)
                return;

            StreamReader reader = new StreamReader(result.Content.ReadAsStream());
            string answer = reader.ReadToEnd();
            OperationHistoryAnswer historyRecord = JsonSerializer.Deserialize<OperationHistoryAnswer>(answer);
            foreach (OperationHistoryElement element in historyRecord.History)
                Item.LoadFromAPI(element);
        }
    }
}
