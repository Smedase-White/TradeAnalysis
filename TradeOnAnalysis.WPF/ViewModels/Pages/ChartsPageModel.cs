using System.Collections.Generic;
using System.Collections.ObjectModel;

using TradeOnAnalysis.Core.Utils.Statistics.Elements;

namespace TradeOnAnalysis.WPF.ViewModels;

public class ChartsPageModel : ViewModelBase
{
    private AccountSelectModel _accountSelect = new();

    private Period _selectionPeriod = Period.Month;
    private Period _pointPeriod = Period.Day;

    private ObservableCollection<ChartModel> _charts = new()
    {
        new("Покупки", s => (s as TradeStatisticElement)!.Buy),
        new("Продажи", s => (s as TradeStatisticElement)!.Sell),
        new("Профит", s => (s as TradeStatisticElement)!.Profit),
        new("Ежедневный профит", s => (s as TradeStatisticElement)!.DailyProfit),
    };

    public ChartsPageModel()
    {
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
        get => new Period[] { Period.Week, Period.Month, Period.HalfYear };
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
            AddAccountChart(Charts[0], account);
            AddAccountChart(Charts[1], account);
            AddAccountChart(Charts[2], account);
            AddAccountChart(Charts[3], account);
        }
    }

    private void AddAccountChart(ChartModel chart, AccountDataModel account)
    {
        chart.Add(account.Statistics!, account.AccountName, SelectionPeriod, PointPeriod);
    }
}