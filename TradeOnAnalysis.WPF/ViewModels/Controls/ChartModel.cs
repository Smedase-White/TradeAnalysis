using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;

using SkiaSharp;

using TradeOnAnalysis.Core.Utils.Statistics;
using TradeOnAnalysis.Core.Utils.Statistics.Elements;

namespace TradeOnAnalysis.WPF.ViewModels;

public class ChartModel : ViewModelBase
{
    public static readonly SKColor LegendColor = new(255, 255, 255);
    public static readonly SKColor GeometryFillColor = new(255, 255, 255);
    public static readonly SKColor[] LinesColors =
    {
        new SKColor(50, 200, 50),
        new SKColor(50, 50, 200),
        new SKColor(200, 50, 50),
        new SKColor(200, 200, 50),
        new SKColor(200, 50, 200),
        new SKColor(50, 200, 200),
    };

    private LabelVisual _title;
    private ObservableCollection<ISeries> _series = new();
    private readonly Func<StatisticElement, double> _selectionFunc;

    public static readonly Axis XAxix = new()
    {
        Labeler = value => new DateTime((long)Math.Abs(value)).ToString("dd MMM"),
        UnitWidth = TimeSpan.FromDays(1).Ticks,
        MinStep = TimeSpan.FromDays(1).Ticks
    };

    public ChartModel(string title, Func<StatisticElement, double> selectionFunc)
    {
        _title = new LabelVisual
        {
            Text = title,
            TextSize = 16,
            Padding = new Padding(15),
            Paint = new SolidColorPaint(LegendColor)
        };
        _selectionFunc = selectionFunc;
    }

    public LabelVisual Title
    {
        get => _title;
        set => ChangeProperty(ref _title, value);
    }

    public ObservableCollection<ISeries> Series
    {
        get => _series;
        set => ChangeProperty(ref _series, value);
    }

    public SolidColorPaint LegendTextPaint { get; set; } = new(LegendColor);

    public Axis[] XAxes { get; set; } = { XAxix };

    public void Add(ObservableCollection<DateTimePoint> points, string title)
    {
        SKColor color = LinesColors[Series.Count % LinesColors.Length];
        Series.Add(CreateLineSeries(points, title, color));
    }

    public void Clear()
    {
        Series.Clear();
    }

    public void Add<StatisticType>(FullStatistics<StatisticType> statistics,
        string title, Period selectPeriod, Period calcPeriod)
        where StatisticType : StatisticElement, new()
    {
        ObservableCollection<DateTimePoint> points = SelectValues(statistics, selectPeriod, calcPeriod);
        Add(points, title);
    }

    private ObservableCollection<DateTimePoint> SelectValues<StatisticType>(
        FullStatistics<StatisticType> statistics,
        Period selectPeriod = Period.Month, Period calcPeriod = Period.Day)
        where StatisticType : StatisticElement, new()
    {
        DateTime endDate = DateTime.Now.Date;
        DateTime startDate = selectPeriod switch
        {
            Period.Week => endDate.AddDays(-7),
            Period.Month => endDate.AddMonths(-1),
            Period.HalfYear => endDate.AddMonths(-6),
            _ => throw new ArgumentException("")
        };

        IEnumerable<StatisticType> displayedData = calcPeriod switch
        {
            Period.Day => statistics.DailyData!,
            Period.Week => statistics.WeeklyData!,
            Period.Month => statistics.MonthlyData!,
            _ => throw new ArgumentException("")
        };

        return new(from data in displayedData
                   where data.Date >= startDate && data.Date <= endDate
                   select new DateTimePoint(data.Date, _selectionFunc(data)));
    }

    private static LineSeries<DateTimePoint> CreateLineSeries(
        ObservableCollection<DateTimePoint> points, string title, SKColor color)
    {
        return new()
        {
            YToolTipLabelFormatter = (chartPoint) => $"{new DateTime((long)chartPoint.Coordinate.SecondaryValue):dd MMMM}: {Math.Round(chartPoint.Coordinate.PrimaryValue, 2)}",
            GeometrySize = 6,
            LineSmoothness = 1,
            Values = points,
            Name = title,
            Stroke = new SolidColorPaint(color, 3) { PathEffect = new DashEffect(new float[] { 6, 4 }) },
            Fill = new SolidColorPaint(color.WithAlpha(110)),
            GeometryStroke = new SolidColorPaint(color, 2),
            GeometryFill = new SolidColorPaint(GeometryFillColor)
        };
    }
}

public enum Period
{
    Day,
    Week,
    Month,
    HalfYear
}