using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TradeOnAnalysis.WPF.ViewModels;

public class AccountsPageModel : ViewModelBase
{
    private ObservableCollection<AccountDataModel> _accounts = new();

    private RelayCommand? _addAccountCommand;

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
        get => _addAccountCommand ??= new(obj =>
        {
            AccountDataModel data = new();
            data.RemoveCommand.AddExecute(obj => { Accounts.Remove(data); });
            Accounts.Add(data);
        });
    }
}
