using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TradeOnAnalysis.WPF.Utils;

namespace TradeOnAnalysis.WPF.ViewModels;

public class AccountsPageModel : ViewModelBase
{
    private const string SaveFilePath = "Accounts.json";

    private ObservableCollection<AccountDataModel> _accounts = new();

    private RelayCommand? _addAccountCommand;
    private RelayCommand? _loadAccountsCommand;
    private RelayCommand? _saveAccountsCommand;

    public AccountsPageModel()
    {

    }

    public ObservableCollection<AccountDataModel> Accounts
    {
        get => _accounts;
        private set => ChangeProperty(ref _accounts, value);
    }

    public IEnumerable<AccountDataModel> LoadedAccounts
    {
        get => Accounts.Where(account => account.Statistics is not null);
    }

    public RelayCommand AddAccountCommand
    {
        get => _addAccountCommand ??= new(obj => AddAccount());
    }

    public RelayCommand LoadAccountsCommand
    {
        get => _loadAccountsCommand ??= new(obj => LoadAccounts());
    }

    public RelayCommand SaveAccountsCommand
    {
        get => _saveAccountsCommand ??= new(obj => SaveAccounts());
    }

    public void AddAccount(AccountDataModel? data = null)
    {
        data ??= new();
        data.RemoveCommand.AddExecute(obj => Accounts.Remove(data));
        Accounts.Add(data);
    }

    public void LoadAccounts()
    {
        AccountsSave? save = JsonSave.Load<AccountsSave>(SaveFilePath);
        if (save == null)
            return;
        Accounts.Clear();
        foreach (AccountSave savedAccount in save.Accounts)
        {
            AccountDataModel account = new() { AccountName = savedAccount.Name, MarketApi = savedAccount.MarketApi };
            AddAccount(account);
            account.LoadCommand.Execute(this);
        }
    }

    public void SaveAccounts()
    {
        AccountsSave save = new()
        {
            Accounts = Accounts.Select(
                account => new AccountSave() { Name = account.AccountName, MarketApi = account.MarketApi }).ToList()
        };
        JsonSave.Save(save, SaveFilePath);
    }
}
