using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SkiaSharp;

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

using static TradeAnalysis.Core.Utils.TimeUtils;

namespace TradeAnalysis.WPF.ViewModels;

using static ChartUtils;

public class ChartsPageModel : ViewModelBase
{
    private AccountSelectModel _accountSelect = new();

    private Period _pointPeriod = Period.Day;
    private int _selectionLength = 1;
    private Period _selectionPeriod = Period.Month;

    private PeriodSelection _periodSelection = PeriodSelection.Last;

    private readonly ObservableCollection<ChartModel> _accountsPeriodicityCharts = new()
    {
        new("Сумма покупок", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.Buy)),
        new("Количество покупок", LegendValueType.Sum, ChartUnit.Count,
            SelectValue<OperationStatisticElement>(e => e.BuyCount)),
        new("Сумма продаж", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.Sell)),
        new("Количество продаж", LegendValueType.Sum, ChartUnit.Count,
            SelectValue<OperationStatisticElement>(e => e.SellCount)),
        new("Транзакции", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.Transaction)),
        new("Депозит предметами", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.DepositInItems)),
        new("Профит", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<TradeStatisticElement>(e => e.Profit)),
        new("Ежедневный профит", LegendValueType.Avg, ChartUnit.Currency,
            SelectValue<TradeStatisticElement>(e => e.HourlyProfit)),
        new("Длительность продажи", LegendValueType.Avg, ChartUnit.Days,
            SelectValue<TradeStatisticElement>(e => e.SellDuration / 24, true)),
        new("Стоимость инвентаря", LegendValueType.Last, ChartUnit.Currency,
            SelectValue<AccountStatisticElement>(e => e.Cost)),
        new("Профит от стоимости", LegendValueType.Avg, ChartUnit.Percent,
            SelectValue<AccountStatisticElement>(e => e.Profit / e.Prev?.Cost * 100)),
    };

    private readonly ObservableCollection<ChartModel> _accountsSeasonalityCharts = new()
    {
        new("Покупки в определённое время", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.Buy)),
        new("Продажи в определённое время", LegendValueType.Sum, ChartUnit.Currency,
            SelectValue<OperationStatisticElement>(e => e.Sell)),
    };

    private readonly ObservableCollection<ChartModel> _marketPeriodicityCharts = new()
    {
        new("Средние цены", LegendValueType.Avg, ChartUnit.Percent,
            SelectValue<MarketStatisticElement>(e => e.Price * 100)),
        new("Среднее число покупок", LegendValueType.Avg, ChartUnit.Count,
            SelectValue<MarketStatisticElement>(e => e.Count))
    };

    private readonly ObservableCollection<ChartModel> _marketSeasonalityCharts = new()
    {
        new("Цены в определённое время", LegendValueType.Avg, ChartUnit.Percent,
            SelectValue<MarketStatisticElement>(e => e.Price * 100)),
    };

    private ObservableCollection<ChartModel> _charts = new();

    public ChartsPageModel()
    {
        AddCharts(_accountsPeriodicityCharts);
        AddCharts(_accountsSeasonalityCharts);
        AddCharts(_marketPeriodicityCharts);
        AddCharts(_marketSeasonalityCharts);

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

    public int SelectionLength
    {
        get => _selectionLength;
        set
        {
            ChangeProperty(ref _selectionLength, value);
            DrawCharts();
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
        get => new Period[] { Period.Day, Period.Week, Period.Month, Period.Year };
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

        ChangeChartAxes(_accountsPeriodicityCharts, SelectionPeriod);
        ChangeChartAxes(_accountsSeasonalityCharts, PointPeriod);
        ChangeChartAxes(_marketPeriodicityCharts, SelectionPeriod);
        ChangeChartAxes(_marketSeasonalityCharts, PointPeriod);

        foreach (AccountDataModel account in _accountSelect.SelectedAccounts)
        {
            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            DrawChartsType(SelectPeriodicityStatistics(account.Account!.Statistics!), account.AccountName, accountColor, _accountsPeriodicityCharts);
            DrawChartsType(SelectSeasonalityStatistics(account.Account!.Statistics!), account.AccountName, accountColor, _accountsSeasonalityCharts);
        }
        foreach (AccountDataModel account in _accountSelect.SelectedAccounts)
        {
            if (account.Account?.MarketStatistics is null)
                continue;

            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            DrawChartsType(SelectPeriodicityStatistics(account.Account.MarketStatistics), account.AccountName, accountColor, _marketPeriodicityCharts);
            DrawChartsType(SelectSeasonalityStatistics(account.Account.MarketStatistics), account.AccountName, accountColor, _marketSeasonalityCharts);
        }
    }

    private static Func<StatisticElement, double?> SelectValue<StatisticType>(Func<StatisticType, double?> selection, bool zeroNullable = false)
        where StatisticType : StatisticElement
    {
        return element =>
        {
            if (element.IsEmpty == true)
                return null;
            StatisticType converted = (element as StatisticType)!;
            double? value = selection(converted);
            if (value == 0 && zeroNullable == true)
                return null;
            return value;
        };
    }

    private void AddCharts(IEnumerable<ChartModel> charts)
    {
        foreach (ChartModel chart in charts)
            _charts.Add(chart);
    }

    private static void ChangeChartAxes(IEnumerable<ChartModel> charts, Period period)
    {
        foreach (ChartModel chart in charts)
            chart.XAxes = new[] { GetAxisByPeriod(period) };
    }

    private static void DrawChartsType<StatisticType>(IEnumerable<StatisticType> statistics, string title, SKColor color, IEnumerable<ChartModel> charts)
        where StatisticType : StatisticElement, new()
    {
        foreach (ChartModel chart in charts)
            chart.Add(statistics, title, color);
    }

    private IEnumerable<StatisticType> SelectPeriodicityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return SelectStatistics(statistics.CalcPeriodData(PointPeriod)!).Data!;
    }

    private IEnumerable<StatisticType> SelectSeasonalityStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return SelectStatistics(statistics).CalcSeasonData(PointPeriod)!.Data!;
    }

    private Statistics<StatisticType> SelectStatistics<StatisticType>(Statistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return PeriodSelection switch
        {
            PeriodSelection.Last => statistics.SelectDataPeriod(DateTime.Now.Ceiling(PointPeriod).AddPeriod(SelectionPeriod, -_selectionLength), DateTime.Now.Ceiling(PointPeriod))!,
            PeriodSelection.Current => statistics.SelectDataPeriod(DateTime.Now.Floor(SelectionPeriod).AddPeriod(SelectionPeriod, -_selectionLength + 1), DateTime.Now.Ceiling(SelectionPeriod))!,
            _ => throw new NotImplementedException(),
        };
    }
}

public enum PeriodSelection
{
    Last,
    Current
}