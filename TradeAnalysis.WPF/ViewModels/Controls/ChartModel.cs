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

using static TradeAnalysis.WPF.ViewModels.ChartUtils;

namespace TradeAnalysis.WPF.ViewModels;

public class ChartModel : ViewModelBase
{
    private readonly LabelVisual _title;
    private readonly LegendValueType _legendValueType;
    private readonly ChartUnit _unit;
    private readonly Axis[] _yAxes;
    private readonly Func<StatisticElement, double?> _selectionFunc;

    public Axis[] _xAxes = { DayAxis };

    private ObservableCollection<ISeries> _series = new();

    public ChartModel(string title, LegendValueType legendValueType, ChartUnit unit, Func<StatisticElement, double?> selectionFunc)
    {
        _title = CreateTitle(title);
        _legendValueType = legendValueType;
        _unit = unit;
        _yAxes = new[] { GetAxisByValueType(unit) };
        _selectionFunc = selectionFunc;
    }

    public LabelVisual Title
    {
        get => _title;
    }

    public LegendValueType LegendValueType
    {
        get => _legendValueType;
    }

    public Axis[] YAxes
    {
        get => _yAxes;
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

    public ObservableCollection<ISeries> Series
    {
        get => _series;
        set => ChangeProperty(ref _series, value);
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
        title = $"{title} ({Math.Round(_legendValueType switch
        {
            LegendValueType.Sum => statistics.Sum(e => _selectionFunc(e)) ?? 0,
            LegendValueType.Avg => statistics.Average(e => _selectionFunc(e)) ?? 0,
            LegendValueType.Last => _selectionFunc(statistics.Last()) ?? 0,
            _ => throw new NotImplementedException(),
        }, 2)} {GetUnitString(_unit)})";
        Series.Add(CreateLineSeries(points, title, color));
    }
}