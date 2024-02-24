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

    private HttpStatusCode _status = HttpStatusCode.NoContent;
    private HttpStatusCode _marketStatus = HttpStatusCode.NoContent;

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
                Status = Account?.Statistics is null ? HttpStatusCode.NoContent : HttpStatusCode.NonAuthoritativeInformation;
                MarketStatus = Account?.MarketStatistics is null ? HttpStatusCode.NoContent : HttpStatusCode.NonAuthoritativeInformation;
            }
        }
    }

    public ColorModel Color
    {
        get => _color;
        set => ChangeProperty(ref _color, value);
    }

    public HttpStatusCode Status
    {
        get => _status;
        private set => ChangeProperty(ref _status, value);
    }

    public HttpStatusCode MarketStatus
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
        Status = HttpStatusCode.Processing;
        Account = new(MarketApis);
        HttpStatusCode statusCode = Account.LoadHistory();
        Status = statusCode;
        if (statusCode != HttpStatusCode.OK)
        {
            Account = null;
            return;
        }
        if (Account.ItemsHistory!.Count == 0)
        {
            Status = HttpStatusCode.NoContent;
            Account = null;
            return;
        }
    }

    public void ParseMarket()
    {
        if (Account is null)
            return;

        MarketStatus = HttpStatusCode.Processing;
        Account.ParseItems();
        MarketStatus = HttpStatusCode.OK;
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
