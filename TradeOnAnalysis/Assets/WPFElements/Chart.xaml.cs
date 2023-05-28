using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TradeOnAnalysis.Assets.WPFElements
{
    /// <summary>
    /// Логика взаимодействия для ProfitChart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public Chart()
        {
            DataContext = this;
            InitializeComponent();
        }

        public List<ISeries> Series { get; set; } = new();

        public ISeries[] CatesianSeries { get; set; } =
        {
            new LineSeries<int> { Values = new int[] { 1, 5, 4, 6 } },
            new ColumnSeries<int> { Values = new int[] { 4, 8, 2, 4 } }
        };

        private void DisplayValues(IEnumerable<double> values, string? title = null, SKColor? color = null)
        {
            LineSeries<double> line = new()
            {
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

            Series.Add(line);
            ChartElement.LegendPosition = LiveChartsCore.Measure.LegendPosition.Top;
            ChartElement.LegendTextPaint = new SolidColorPaint(new SKColor(255, 255, 255));
            ChartElement.Series = Series.ToArray();
        }

        private List<double> CalcDailyValues(IEnumerable<Item> items, Func<Item, DateTime, bool> predicate, Func<Item, double> selection, DateTime startDate, DateTime endDate)
        {
            List<double> values = new();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                IEnumerable<double> dailyValues = from item in items
                                                  where predicate(item, date)
                                                  select selection(item);
                values.Add(dailyValues.Sum());
            }
            return values;
        }

        public void DisplayBuys(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            List<double> values = CalcDailyValues(items,
                (item, date) => date == item.BuyInfo!.Date.Date,
                item => item.BuyInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(values, "Покупки", new SKColor(244, 67, 54));
        }

        public void DisplaySells(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            List<double> values = CalcDailyValues(items,
                (item, date) => date == item.SellInfo!.Date.Date,
                item => item.SellInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(values, "Продажи", new SKColor(33, 150, 243));
        }

        public void DisplayProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            List<double> values = CalcDailyValues(items,
                (item, date) => date == item.SellInfo!.Date.Date,
                item => item.SellInfo!.Price - item.BuyInfo!.Price,
                startDate.Date, endDate.Date);
            DisplayValues(values, "Профит", new SKColor(15, 255, 131));
        }

        public void DisplayDailyProfit(IEnumerable<Item> items, DateTime startDate, DateTime endDate)
        {
            List<double> values = CalcDailyValues(items,
                (item, date) => item.BuyInfo!.Date.Date < date && date <= item.SellInfo!.Date.Date,
                item => (item.SellInfo!.Price - item.BuyInfo!.Price) / (item.SellInfo!.Date - item.BuyInfo!.Date).Days,
                startDate.Date, endDate.Date);
            DisplayValues(values, "Ежедневный профит", new SKColor(15, 255, 131));
        }

        public void Clear()
        {
            Series.Clear();
        }
    }

    public enum Month
    {
        Jan = 1,
        Feb = 2,
        Mar = 3,
        Apr = 4,
        Jun = 5,
        Jul = 6,
        Aug = 7,
        Sep = 8,
        Oct = 9,
        Nov = 10,
        Dec = 11
    }
}
