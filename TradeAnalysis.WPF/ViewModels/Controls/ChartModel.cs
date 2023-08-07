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

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.WPF.ViewModels;

public class ChartModel : ViewModelBase
{
    public static readonly SKColor LegendColor = new(255, 255, 255);
    public static readonly SKColor GeometryFillColor = new(255, 255, 255);

    private LabelVisual _title;
    private ObservableCollection<ISeries> _series = new();
    private readonly Func<StatisticElement, double?> _selectionFunc;

    public static Axis HourAxis => new()
    {
        Labeler = value => new DateTime((long)Math.Abs(value)).ToString("HH"),
        UnitWidth = TimeSpan.FromHours(2).Ticks,
        MinStep = TimeSpan.FromHours(2).Ticks
    };
    public static Axis DayAxis => new()
    {
        Labeler = value => new DateTime((long)Math.Abs(value)).ToString("dd MMM"),
        UnitWidth = TimeSpan.FromDays(1).Ticks,
        MinStep = TimeSpan.FromDays(1).Ticks
    };
    public static Axis MonthAxis => new()
    {
        Labeler = value => new DateTime((long)Math.Abs(value)).ToString("MMM yy"),
        UnitWidth = TimeSpan.FromDays(30).Ticks,
        MinStep = TimeSpan.FromDays(30).Ticks
    };
    public Axis[] _xAxes = { DayAxis };

    public ChartModel(string title, Func<StatisticElement, double?> selectionFunc)
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

    public Func<StatisticElement, double?> SelectionFunc
    {
        get => _selectionFunc;
    }

    public SolidColorPaint LegendTextPaint { get; set; } = new(LegendColor);

    public Axis[] XAxes
    {
        get => _xAxes;
        set => ChangeProperty(ref _xAxes, value);
    }

    public void Clear()
    {
        Series.Clear();
    }

    public void Add<StatisticType>(IEnumerable<StatisticType> statistics, string title, SKColor color)
        where StatisticType : StatisticElement, new()
    {
        ObservableCollection<DateTimePoint> points =
            new(statistics.Select(data => new DateTimePoint(data.Time, _selectionFunc(data))));
        Series.Add(CreateLineSeries(points, title, color));
    }

    public void ChangeAxes(Period period)
        => _ = period switch
        {
            Period.Hour => XAxes = new Axis[] { HourAxis },
            Period.Day => XAxes = new Axis[] { HourAxis },
            Period.Week => XAxes = new Axis[] { DayAxis },
            Period.Month => XAxes = new Axis[] { DayAxis },
            Period.HalfYear => XAxes = new Axis[] { MonthAxis },
            Period.FourYears => XAxes = new Axis[] { MonthAxis },
            _ => throw new NotImplementedException(),
        };


    private static LineSeries<DateTimePoint> CreateLineSeries(
        ObservableCollection<DateTimePoint> points, string title, SKColor color)
    {
        return new()
        {
            YToolTipLabelFormatter = (chartPoint) => $"{Math.Round(chartPoint.Coordinate.PrimaryValue, 2)}",
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