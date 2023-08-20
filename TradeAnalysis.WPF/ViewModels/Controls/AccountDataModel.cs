using System.Net;
using System.Threading.Tasks;

using TradeAnalysis.Core.Utils;
using TradeAnalysis.Core.Utils.Saves;

namespace TradeAnalysis.WPF.ViewModels;

public class AccountDataModel : ViewModelBase
{
    private string _accountName = "";
    private string _marketApi = "";
    private ColorModel _color = new();

    private string _status = "Empty";

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

    public string MarketApi
    {
        get => _marketApi;
        set
        {
            if (ChangeProperty(ref _marketApi, value))
                Status = Account?.Statistics is null ? "Empty" : "Other";
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
        Status = "Load";
        Account = new(MarketApi);
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

        Account.ParseItems();
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
