using System.Net;

using TradeAnalysis.Core.Utils.Saves;
using TradeAnalysis.Core.Utils.Statistics;

namespace TradeAnalysis.WPF.ViewModels;

public class AccountDataModel : ViewModelBase
{
    private string _accountName = "";
    private string _marketApi = "";

    private ColorModel _color = new();

    private string _status = "Empty";
    private AccountStatistics? _stats;

    private RelayCommand? _loadCommand;
    private RelayCommand? _removeCommand;

    public AccountDataModel()
    {

    }

    public string AccountName
    {
        get => _accountName;
        set => ChangeProperty(ref _accountName, value);
    }

    public string MarketApi
    {
        get => _marketApi;
        set
        {
            if (ChangeProperty(ref _marketApi, value))
                Status = Statistics is null ? "Empty" : "Other";
        }
    }

    public ColorModel Color
    {
        get => _color;
        set => ChangeProperty(ref _color, value);
    }

    public string Status
    {
        get => _status;
        private set => ChangeProperty(ref _status, value);
    }

    public AccountStatistics? Statistics
    {
        get => _stats;
        private set => ChangeProperty(ref _stats, value);
    }

    public RelayCommand LoadCommand
    {
        get => _loadCommand ??= new(obj => LoadStatistics());
        set => ChangeProperty(ref _loadCommand, value);
    }

    public RelayCommand RemoveCommand
    {
        get => _removeCommand ??= new();
        set => ChangeProperty(ref _removeCommand, value);
    }

    public void LoadStatistics()
    {
        Statistics = new(MarketApi);
        HttpStatusCode statusCode = Statistics.LoadHistory();
        Status = $"{statusCode}";
        if (statusCode != HttpStatusCode.OK)
            return;
        if (Statistics.History!.Count == 0)
        {
            Status = "Empty";
            Statistics = null;
            return;
        }
        Statistics.CalcData();
    }

    public AccountSave GetSave()
    {
        return new AccountSave()
        {
            Name = AccountName,
            MarketApi = MarketApi,
            Color = Color.Hex
        };
    }

    public void LoadSave(AccountSave save)
    {
        AccountName = save.Name;
        MarketApi = save.MarketApi;
        Color.Hex = save.Color;
    }
}
