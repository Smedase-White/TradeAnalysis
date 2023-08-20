using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;

using SkiaSharp;

using TradeAnalysis.Core.Utils;

namespace TradeAnalysis.WPF.ViewModels;

public enum ChartUnit
{
    Currency,
    Count,
    Percent
}

public enum LegendValueType
{
    Sum,
    Avg,
    Last
}

public static class ChartUtils
{
    public static readonly SKColor LegendColor = new(255, 255, 255);
    public static readonly SKColor GeometryFillColor = new(255, 255, 255);

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
    public static Axis GetAxisByPeriod(Period period)
        => period switch
        {
            Period.Hour => HourAxis,
            Period.Day => HourAxis,
            Period.Week => DayAxis,
            Period.Month => DayAxis,
            Period.HalfYear => MonthAxis,
            Period.FourYears => MonthAxis,
            _ => throw new NotImplementedException(),
        };

    public static Axis GetAxisByValueType(ChartUnit unit)
        => new() { Labeler = value => $"{Math.Round(value, 2)} {GetUnitString(unit)}" };
    public static string GetUnitString(ChartUnit unit)
        => unit switch
        {
            ChartUnit.Currency => "₽",
            ChartUnit.Count => "шт.",
            ChartUnit.Percent => "%",
            _ => throw new NotImplementedException(),
        };

    public static LabelVisual CreateTitle(string title)
        => new()
        {
            Text = title,
            TextSize = 16,
            Padding = new Padding(15),
            Paint = new SolidColorPaint(LegendColor)
        };

    public static LineSeries<DateTimePoint> CreateLineSeries(
        ObservableCollection<DateTimePoint> points, string title, SKColor color)
        => new()
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
