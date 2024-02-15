using System.Net;
using System.Threading.Tasks;

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Saves;

namespace TradeAnalysis.WPF.ViewModels;

public class AccountDataModel : ViewModelBase
{
    private string _accountName = "";
    private string _marketApis = "";
    private ColorModel _color = new();

    private string _status = "Empty";
    private string _marketStatus = "Empty";

    private RelayCommand? _loadCommand;
    private RelayCommand? _parseCommand;
    private RelayCommand? _removeCommand;

    private Account? _account;

    public AccountDataModel()
    {

    }

    public string AccountName
    {
        get => _accountName;
        set => ChangeProperty(ref _accountName, value);
    }

    public string MarketApis
    {
        get => _marketApis;
        set
        {
            if (ChangeProperty(ref _marketApis, value))
            {
                Status = Account?.Statistics is null ? "Empty" : "Other";
                MarketStatus = Account?.MarketStatistics is null ? "Empty" : "Other";
            }
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

    public string MarketStatus
    {
        get => _marketStatus;
        private set => ChangeProperty(ref _marketStatus, value);
    }

    public Account? Account
    {
        get => _account;
        private set => ChangeProperty(ref _account, value);
    }

    public RelayCommand LoadCommand
    {
        get => _loadCommand ??= new(obj => Task.Run(() => LoadAccount()));
        set => ChangeProperty(ref _loadCommand, value);
    }

    public RelayCommand ParseCommand
    {
        get => _parseCommand ??= new(obj => Task.Run(() => ParseMarket()));
        set => ChangeProperty(ref _parseCommand, value);
    }

    public RelayCommand RemoveCommand
    {
        get => _removeCommand ??= new();
        set => ChangeProperty(ref _removeCommand, value);
    }

    public void LoadAccount()
    {
        if (MarketApis.Equals(""))
            return;
        Status = "Load";
        Account = new(MarketApis);
        HttpStatusCode statusCode = Account.LoadHistory();
        Status = $"{statusCode}";
        if (statusCode != HttpStatusCode.OK)
        {
            Account = null;
            return;
        }
        if (Account.ItemsHistory!.Count == 0)
        {
            Status = "Empty";
            Account = null;
            return;
        }
    }

    public void ParseMarket()
    {
        if (Account is null)
            return;

        MarketStatus = "Load";
        Account.ParseItems();
        MarketStatus = "OK";
    }

    public AccountSave GetSave()
    {
        return new AccountSave()
        {
            Name = AccountName,
            MarketApi = MarketApis,
            Color = Color.Hex
        };
    }

    public void LoadSave(AccountSave save)
    {
        AccountName = save.Name;
        MarketApis = save.MarketApi;
        Color.Hex = save.Color;
    }
}
