using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SkiaSharp;

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.Statistics.Base.StatisticsUtils;
using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.WPF.ViewModels;

public class ChartsPageModel : ViewModelBase
{
    private AccountSelectModel _accountSelect = new();

    private Period _selectionPeriod = Period.Month;
    private Period _pointPeriod = Period.Day;

    private PeriodSelection _periodSelection = PeriodSelection.Last;

    private readonly ObservableCollection<ChartModel> _accountsPeriodicityCharts = new()
    {
        new("Покупки",
            e => (e is TradeStatisticElement t) ? t.IsEmpty == false ? t.Buy: null : 0),
        new("Продажи",
            e => (e is TradeStatisticElement t) ? t.IsEmpty == false ? t.Sell: null : 0),
        new("Профит",
            e =>(e is TradeStatisticElement t) ? t.IsEmpty == false ? t.Profit : null : 0),
        new("Ежедневный профит",
            e =>(e is TradeStatisticElement t) ? t.IsEmpty == false ? t.HourlyProfit: null : 0),
    };

    private readonly ObservableCollection<ChartModel> _accountsSeasonalityCharts = new()
    {
        new("Покупки в определённое время",
            e => (e as OperationStatisticElement)!.Buy),
        new("Продажи в определённое время",
            e => (e as OperationStatisticElement)!.Sell),
    };

    private ObservableCollection<ChartModel> _charts = new();

    public ChartsPageModel()
    {
        foreach (ChartModel chart in _accountsPeriodicityCharts)
            _charts.Add(chart);
        foreach (ChartModel chart in _accountsSeasonalityCharts)
            _charts.Add(chart);

        AccountSelect.CloseCommand.AddExecute(obj => { DrawCharts(); });
    }

    public AccountSelectModel AccountSelect
    {
        get => _accountSelect;
        set
        {
            ChangeProperty(ref _accountSelect, value);
            _accountSelect.CloseCommand.AddExecute(obj => { DrawCharts(); });
        }
    }

    public Period SelectionPeriod
    {
        get => _selectionPeriod;
        set
        {
            ChangeProperty(ref _selectionPeriod, value);
            DrawCharts();
        }
    }

    public IEnumerable<Period> SelectionPeriodValues
    {
        get => new Period[] { Period.Day, Period.Week, Period.Month, Period.HalfYear, Period.FourYears };
    }

    public Period PointPeriod
    {
        get => _pointPeriod;
        set
        {
            ChangeProperty(ref _pointPeriod, value);
            DrawCharts();
        }
    }

    public IEnumerable<Period> PointPeriodValues
    {
        get => new Period[] { Period.Hour, Period.Day, Period.Week, Period.Month };
    }

    public PeriodSelection PeriodSelection
    {
        get => _periodSelection;
        set
        {
            ChangeProperty(ref _periodSelection, value);
            DrawCharts();
        }
    }

    public IEnumerable<PeriodSelection> PeriodSelectionValues
    {
        get => new PeriodSelection[] { PeriodSelection.Last, PeriodSelection.Current };
    }

    public ObservableCollection<ChartModel> Charts
    {
        get => _charts;
        set => ChangeProperty(ref _charts, value);
    }

    public void DrawCharts()
    {

        foreach (ChartModel chart in Charts)
            chart.Clear();

        foreach (ChartModel periodicityChart in _accountsPeriodicityCharts)
            periodicityChart.ChangeAxes(SelectionPeriod);
        foreach (ChartModel seasonalityChart in _accountsSeasonalityCharts)
            seasonalityChart.ChangeAxes(PointPeriod);

        foreach (AccountDataModel account in _accountSelect.SelectedAccounts)
        {
            IEnumerable<TradeStatisticElement> periodicityStatistics = SelectPeriodicityStatistics(account.Account!.TradeStatistics!);
            IEnumerable<OperationStatisticElement> seasonalityStatistics = SelectSeasonalityStatistics(account.Account!.TradeStatistics!);
            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            foreach (ChartModel accountChart in _accountsPeriodicityCharts)
                accountChart.Add(periodicityStatistics, account.AccountName, accountColor);
            foreach (ChartModel accountChart in _accountsSeasonalityCharts)
                accountChart.Add(seasonalityStatistics, account.AccountName, accountColor);
        }
    }

    private IEnumerable<StatisticType> SelectPeriodicityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return CalcPeriodData(SelectStatistics(statistics), PointPeriod);
    }

    private IEnumerable<StatisticType> SelectSeasonalityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return CalcSeasonData(SelectStatistics(statistics), PointPeriod);
    }

    private SortedSet<StatisticType> SelectStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return PeriodSelection switch
        {
            PeriodSelection.Last => statistics.SelectDataPeriod(DateTime.Now.Ceiling(PointPeriod).AddPeriod(SelectionPeriod, -1), DateTime.Now.Ceiling(PointPeriod))!.Data!,
            PeriodSelection.Current => statistics.SelectDataPeriod(DateTime.Now.Floor(SelectionPeriod), DateTime.Now.Ceiling(SelectionPeriod))!.Data!,
            _ => throw new NotImplementedException(),
        };
    }
}

public enum PeriodSelection
{
    Last,
    Current
}