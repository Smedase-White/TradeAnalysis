using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using LiveChartsCore.SkiaSharpView;

using SkiaSharp;
using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Statistics.Base;
using TradeAnalysis.Core.Utils.Statistics.Elements;

namespace TradeAnalysis.WPF.ViewModels;

public class ChartsPageModel : ViewModelBase
{
    private AccountSelectModel _accountSelect = new();

    private Period _selectionPeriod = Period.Month;
    private Period _pointPeriod = Period.Day;

    private ObservableCollection<ChartModel> _accountsPeriodicityCharts = new()
    {
        new("Покупки", s => (s as TradeStatisticElement)!.Buy),
        new("Продажи", s => (s as TradeStatisticElement)!.Sell),
        new("Профит", s => (s as TradeStatisticElement)!.Profit),
        new("Ежедневный профит", s => (s as TradeStatisticElement)!.HourlyProfit),
    };

    private ObservableCollection<ChartModel> _accountsSeasonalityCharts = new()
    {
        new("Покупки в определённое время", s => (s as OperationStatisticElement)!.Buy),
        new("Продажи в определённое время", s => (s as OperationStatisticElement)!.Sell),
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

    public ObservableCollection<ChartModel> Charts
    {
        get => _charts;
        set => ChangeProperty(ref _charts, value);
    }

    public void DrawCharts()
    {

        foreach (ChartModel chart in Charts)
            chart.Clear();

        foreach (AccountDataModel account in _accountSelect.SelectedAccounts)
        {
            IEnumerable<TradeStatisticElement> periodicityStatistics = SelectPeriodicityStatistics(account.Account!.TradeStatistics!);
            IEnumerable<OperationStatisticElement> seasonalityStatistics = SelectSeasonalityStatistics(account.Account!.TradeStatistics!);
            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            foreach (ChartModel accountChart in _accountsPeriodicityCharts)
            {
                accountChart.Add(periodicityStatistics, account.AccountName, accountColor);
                accountChart.XAxes = SelectionPeriod switch
                {
                    Period.Hour => new Axis[] { ChartModel.HourAxis },
                    _ => new Axis[] { ChartModel.DayAxis }
                };
            }
            foreach (ChartModel accountChart in _accountsSeasonalityCharts)
            {
                accountChart.Add(seasonalityStatistics, account.AccountName, accountColor);
                accountChart.XAxes = SelectionPeriod switch
                {
                    Period.Day => new Axis[] { ChartModel.HourAxis },
                    _ => new Axis[] { ChartModel.DayAxis }
                };
            }
        }
    }

    private IEnumerable<StatisticType> SelectPeriodicityStatistics<StatisticType>(PeriodicityStatistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        DateTime endDate = DateTime.Now.AddHours(6);
        DateTime startDate = SelectionPeriod switch
        {
            Period.Day => endDate.AddDays(-1),
            Period.Week => endDate.AddDays(-7),
            Period.Month => endDate.AddMonths(-1),
            Period.HalfYear => endDate.AddMonths(-6),
            Period.FourYears => endDate.AddYears(-4),
            _ => throw new ArgumentException("")
        };

        IEnumerable<StatisticType> displayedData = PointPeriod switch
        {
            Period.Hour => statistics.HourlyData!,
            Period.Day => statistics.DailyData!,
            Period.Week => statistics.WeeklyData!,
            Period.Month => statistics.MonthlyData!,
            _ => throw new ArgumentException("")
        };

        return displayedData.Where(data => data.Time >= startDate && data.Time <= endDate);
    }

    private IEnumerable<StatisticType> SelectSeasonalityStatistics<StatisticType>(SeasonalityStatistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        return SelectionPeriod switch
        {
            Period.Day => statistics.HourOfDayData!,
            Period.Week => statistics.DayOfWeekData!,
            _ => new List<StatisticType>()
        };
    }
}