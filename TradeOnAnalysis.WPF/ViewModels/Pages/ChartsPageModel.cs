using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using SkiaSharp;

using TradeOnAnalysis.Core.Utils.Statistics;
using TradeOnAnalysis.Core.Utils.Statistics.Elements;

namespace TradeOnAnalysis.WPF.ViewModels;

public class ChartsPageModel : ViewModelBase
{
    private AccountSelectModel _accountSelect = new();

    private Period _selectionPeriod = Period.Month;
    private Period _pointPeriod = Period.Day;

    private ObservableCollection<ChartModel> _accountsCharts = new()
    {
        new("Покупки", s => (s as TradeStatisticElement)!.Buy),
        new("Продажи", s => (s as TradeStatisticElement)!.Sell),
        new("Профит", s => (s as TradeStatisticElement)!.Profit),
        new("Ежедневный профит", s => (s as TradeStatisticElement)!.DailyProfit),
    };

    private ObservableCollection<ChartModel> _charts = new();

    public ChartsPageModel()
    {
        foreach (ChartModel chart in _accountsCharts)
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
        get => new Period[] { Period.Week, Period.Month, Period.HalfYear, Period.FourYears };
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
        get => new Period[] { Period.Day, Period.Week, Period.Month };
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
            IEnumerable<TradeStatisticElement> accountStatistics = SelectStatistics(account.Statistics!);
            SKColor accountColor = new(account.Color.Red, account.Color.Green, account.Color.Blue);
            foreach (ChartModel accountChart in _accountsCharts)
                accountChart.Add(accountStatistics, account.AccountName, accountColor);
        }
    }

    private IEnumerable<StatisticType> SelectStatistics<StatisticType>(FullStatistics<StatisticType> statistics)
        where StatisticType : StatisticElement, new()
    {
        DateTime endDate = DateTime.Now.Date;
        DateTime startDate = SelectionPeriod switch
        {
            Period.Week => endDate.AddDays(-7),
            Period.Month => endDate.AddMonths(-1),
            Period.HalfYear => endDate.AddMonths(-6),
            Period.FourYears => endDate.AddYears(-4),
            _ => throw new ArgumentException("")
        };

        IEnumerable<StatisticType> displayedData = PointPeriod switch
        {
            Period.Day => statistics.DailyData!,
            Period.Week => statistics.WeeklyData!,
            Period.Month => statistics.MonthlyData!,
            _ => throw new ArgumentException("")
        };

        return displayedData.Where(data => data.Date >= startDate && data.Date <= endDate);
    }
}

public enum Period
{
    Day,
    Week,
    Month,
    HalfYear,
    FourYears
}