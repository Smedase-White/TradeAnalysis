using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace TradeOnAnalysis.Assets.WPFElements
{
    /// <summary>
    /// Логика взаимодействия для ProfitChart.xaml
    /// </summary>
    public partial class Charts : UserControl
    {
        public static SolidColorPaint LegendTextPaint { get; set; } = new(new SKColor(255, 255, 255));

        public Charts()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<ISeries> Series { get; set; } = new();

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("dd MMM"),
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks
            }
        };

        public ISeries[] CatesianSeries { get; set; } =
        {
            new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
            new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
        };

        private static void DisplayValues(CartesianChart chart, ObservableCollection<DateTimePoint> values, string? title = null, SKColor? color = null)
        {
            LineSeries<DateTimePoint> line = new()
            {
                TooltipLabelFormatter = (chartPoint) => $"{new DateTime((long)chartPoint.SecondaryValue):dd MMMM}: {chartPoint.PrimaryValue}",
                Values = values,
                GeometrySize = 6,
                LineSmoothness = 1,
            };
            if (title != null)
                line.Name = title;
            if (color != null)
            {
                line.Stroke = new SolidColorPaint(color.Value, 3)
                {
                    PathEffect = new DashEffect(new float[] { 6, 4 })
                };
                line.Fill = new SolidColorPaint(color.Value.WithAlpha(110));
                line.GeometryStroke = new SolidColorPaint(color.Value, 2);
                line.GeometryFill = new SolidColorPaint(new SKColor(255, 255, 255));
            }

            chart.Series = chart.Series.Concat(new ISeries[] { line });
        }

        private static ObservableCollection<DateTimePoint> CalcDailyValues(IEnumerable<Item> items, Func<Item, DateTime, bool> predicate, Func<Item, double> selection, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = new();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                IEnumerable<double> dailyValues = from item in items
                                                  where predicate(item, date)
                                                  select selection(item);
                values.Add(new(date, Math.Round(dailyValues.Sum(), 2)));
            }
            return values;
        }

        private static void Clear(CartesianChart chart)
        {
            chart.Series = Array.Empty<LineSeries<DateTimePoint>>();
        }

        public void DisplayBuys(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(items,
                (item, date) => date == item.BuyInfo!.Date.Date,
                item => item.BuyInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(BuySellChart, values, "Покупки", new SKColor(33, 150, 243));
        }

        public void DisplaySells(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(items,
                (item, date) => date == item.SellInfo!.Date.Date,
                item => item.SellInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(BuySellChart, values, "Продажи", new SKColor(244, 67, 54));
        }

        public void DisplayProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(items,
                (item, date) => date == item.SellInfo!.Date.Date,
                item => item.SellInfo!.Price - item.BuyInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(ProfitChart, values, "Профит", new SKColor(15, 255, 131));
        }

        public void DisplayDailyProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(items,
                (item, date) => item.BuyInfo!.Date.Date < date && date <= item.SellInfo!.Date.Date,
                item => (item.SellInfo!.Price - item.BuyInfo!.Price) / (item.SellInfo!.Date - item.BuyInfo!.Date).Days,
                startDate.Date, endDate.Date);
            DisplayValues(DailyProfitChart, values, "Ежедневный профит", new SKColor(15, 255, 131));
        }

        public void DisplayAll(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            Clear(BuySellChart);
            Clear(ProfitChart);
            Clear(DailyProfitChart);

            DisplayBuys(items, startDate, endDate);
            DisplaySells(items, startDate, endDate);
            DisplayProfit(items, startDate, endDate);
            DisplayDailyProfit(items, startDate, endDate);
        }
    }
}
