﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using SkiaSharp;

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

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
            e => (e is OperationStatisticElement t) ? t.IsEmpty == false ? t.Buy: null : 0),
        new("Продажи",
            e => (e is OperationStatisticElement t) ? t.IsEmpty == false ? t.Sell: null : 0),
        new("Транзакции",
            e => (e is OperationStatisticElement t) ? t.IsEmpty == false ? t.Transaction : null : 0),
        new("Депозит предметами",
            e => (e is OperationStatisticElement t) ? t.IsEmpty == false ? t.DepositInItems : null : 0),
        new("Профит",
            e => (e is TradeStatisticElement t) ? t.IsEmpty == false ? t.Profit : null : 0),
        new("Ежедневный профит",
            e => (e is TradeStatisticElement t) ? t.IsEmpty == false ? t.HourlyProfit: null : 0),
        new("Стоимость инвентаря",
            e => (e is AccountStatisticElement t) ? t.IsEmpty == false ? t.Cost : null : 0)
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
            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            DrawChartsType(SelectPeriodicityStatistics(account.Account!.Statistics!), account.AccountName, accountColor, _accountsPeriodicityCharts);
            DrawChartsType(SelectSeasonalityStatistics(account.Account!.Statistics!), account.AccountName, accountColor, _accountsSeasonalityCharts);
        }
    }

    private static void DrawChartsType<StatisticType>(IEnumerable<StatisticType> statistics, string title, SKColor color, IEnumerable<ChartModel> charts)
        where StatisticType : StatisticElement, new()
    {
        foreach (ChartModel chart in charts)
            chart.Add(statistics, $"{title} ({Math.Round(statistics.Sum(e => chart.SelectionFunc(e)) ?? 0, 2)} ₽)", color);
    }

    private IEnumerable<StatisticType> SelectPeriodicityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return SelectStatistics(statistics.CalcPeriodData(PointPeriod)!).Data!;
    }

    private IEnumerable<StatisticType> SelectSeasonalityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return SelectStatistics(statistics.CalcSeasonData(PointPeriod)!).Data!;
    }

    private Statistics<StatisticType> SelectStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return PeriodSelection switch
        {
            PeriodSelection.Last => statistics.SelectDataPeriod(DateTime.Now.Ceiling(PointPeriod).AddPeriod(SelectionPeriod, -1), DateTime.Now.Ceiling(PointPeriod))!,
            PeriodSelection.Current => statistics.SelectDataPeriod(DateTime.Now.Floor(SelectionPeriod), DateTime.Now.Ceiling(SelectionPeriod))!,
            _ => throw new NotImplementedException(),
        };
    }
}

public enum PeriodSelection
{
    Last,
    Current
}