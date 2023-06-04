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
    public partial class Charts : UserControl
    {
        public static readonly SKColor LegendColor = new(255, 255, 255);
        public static readonly SKColor BuyChartColor = new(244, 67, 54);
        public static readonly SKColor SellChartColor = new(33, 150, 243);
        public static readonly SKColor ProfitChartColor = new(15, 255, 131);
        public static readonly SKColor DailyProfitChartColor = new(15, 255, 131);
        public static readonly SKColor MarketChartColor = new(163, 73, 163);

        public Charts()
        {
            InitializeComponent();
            DataContext = this;
        }

        public SolidColorPaint LegendTextPaint { get; set; } = new(LegendColor);

        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                Labeler = value => new DateTime((long) value).ToString("dd MMM"),
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromDays(1).Ticks
            }
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

        private static ObservableCollection<DateTimePoint> CalcDailyValues(DateTime startDate, DateTime endDate,
            IEnumerable<Item> items,
            Func<Item, DateTime, bool> predicate,
            Func<Item, DateTime, double> selection,
            Func<IEnumerable<double>, double>? calc = null)
        {
            ObservableCollection<DateTimePoint> values = new();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                IEnumerable<double> dailyValues = from item in items
                                                  where predicate(item, date)
                                                  select selection(item, date);
                double value;
                if (calc == null)
                    value = Math.Round(dailyValues.Sum(), 2);
                else
                    value = calc(dailyValues);
                values.Add(new(date, value));
            }
            return values;
        }


        private static void Clear(CartesianChart chart)
        {
            chart.Series = Array.Empty<LineSeries<DateTimePoint>>();
        }

        public void DisplayBuys(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(startDate.Date, endDate.Date,
                items,
                (item, date) => date == item.BuyInfo!.Date.Date,
                (item, _) => item.BuyInfo!.Price);
            DisplayValues(BuySellChart, values, "Покупки", BuyChartColor);
        }

        public void DisplaySells(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(startDate.Date, endDate.Date,
                items,
                (item, date) => date == item.SellInfo!.Date.Date,
                (item, _) => item.SellInfo!.Price);
            DisplayValues(BuySellChart, values, "Продажи", SellChartColor);
        }

        public void DisplayProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(startDate.Date, endDate.Date,
                items,
                (item, date) => date == item.SellInfo!.Date.Date,
                (item, _) => item.SellInfo!.Price - item.BuyInfo!.Price);
            DisplayValues(BuySellChart, values, "Профит", ProfitChartColor);
        }

        public void DisplayDailyProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(startDate.Date, endDate.Date,
                items,
                (item, date) => item.BuyInfo!.Date.Date < date && date <= item.SellInfo!.Date.Date,
                (item, _) => (item.SellInfo!.Price - item.BuyInfo!.Price) / (item.SellInfo!.Date - item.BuyInfo!.Date).Days);
            DisplayValues(DailyProfitChart, values, "Ежедневный профит", DailyProfitChartColor);
        }

        public void DisplayMarketAnalysis(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            ObservableCollection<DateTimePoint> values = CalcDailyValues(startDate.Date, endDate.Date,
                items,
                (item, date) => item.History?.ContainsKey(date) ?? false,
                (item, date) => item.History![date].AveragePrice / item.AveragePrice!.Value,
                (enumerable) => enumerable.Any() ? enumerable.Sum() / enumerable.Count() : 1);
            DisplayValues(MarketChart, values, "Анализ маркета", MarketChartColor);
        }

        public void DisplayAll(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            Clear(BuySellChart);
            Clear(DailyProfitChart);
            Clear(MarketChart);

            DisplayBuys(items, startDate, endDate);
            DisplaySells(items, startDate, endDate);
            DisplayProfit(items, startDate, endDate);
            DisplayDailyProfit(items, startDate, endDate);
            DisplayMarketAnalysis(items, startDate, endDate);
        }
    }
}
