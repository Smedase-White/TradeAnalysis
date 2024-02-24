using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using TradeAnalysis.Core.Utils.Saves;

namespace TradeAnalysis.WPF.ViewModels;

public class AccountsPageModel : ViewModelBase
{
    private const string SaveFilePath = "Accounts.json";

    private ObservableCollection<AccountDataModel> _accounts = new();

    private RelayCommand? _addAccountCommand;

    public AccountsPageModel()
    {
        LoadAccounts();
    }

    public ObservableCollection<AccountDataModel> Accounts
    {
        get => _accounts;
        private set => ChangeProperty(ref _accounts, value);
    }

    public IEnumerable<AccountDataModel> LoadedAccounts
    {
        get => Accounts.Where(account => account.Status is HttpStatusCode.OK);
    }

    public RelayCommand AddAccountCommand
    {
        get => _addAccountCommand ??= new(obj => Task.Run(() => AddAccount()));
    }

    public void AddAccount(AccountDataModel? data = null)
    {
        data ??= new();
        data.RemoveCommand.AddExecute(obj => Task.Run(() => { SyncRemoveAccount(data); SaveAccounts(); }));
        data.LoadCommand.AddExecute(obj => Task.Run(() => SaveAccounts()));
        SyncAddAccount(data);
        SaveAccounts();
    }

    public void LoadAccounts()
    {
        AccountsSave? save = JsonSave.Load<AccountsSave>(SaveFilePath);
        if (save == null)
            return;
        SyncClearAccount();
        foreach (AccountSave savedAccount in save.Accounts)
        {
            AccountDataModel account = new();
            account.LoadSave(savedAccount);
            AddAccount(account);
            account.LoadCommand.Execute(this);
        }
    }

    public void SaveAccounts()
    {
        AccountsSave save = new()
        {
            Accounts = Accounts.Select(account => account.GetSave()).ToList()
        };
        JsonSave.Save(save, SaveFilePath);
    }

    private void SyncAddAccount(AccountDataModel data)
    {
        App.Current.Dispatcher.Invoke(() => { Accounts.Add(data); });
    }

    private void SyncRemoveAccount(AccountDataModel data)
    {
        App.Current.Dispatcher.Invoke(() => { Accounts.Remove(data); });
    }

    private void SyncClearAccount()
    {
        App.Current.Dispatcher.Invoke(() => { Accounts.Clear(); });
    }
}
