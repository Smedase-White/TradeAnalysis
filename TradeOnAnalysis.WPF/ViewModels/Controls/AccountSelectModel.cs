using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TradeOnAnalysis.WPF.ViewModels;

public class AccountSelectModel : ViewModelBase
{
    private ObservableCollection<SelectionViewModel> _accountsSelections = new();

    private RelayCommand? _closeCommand;

    public AccountSelectModel()
    {

    }

    public ObservableCollection<SelectionViewModel> AccountsSelections
    {
        get => _accountsSelections;
        set => ChangeProperty(ref _accountsSelections, value);
    }

    public IEnumerable<AccountDataModel> SelectedAccounts
    {
        get => from selection in AccountsSelections
               where selection.IsSelected == true
               select selection.Data;
    }

    public RelayCommand CloseCommand
    {
        get => _closeCommand ??= new();
        set => ChangeProperty(ref _closeCommand, value);
    }

    public void UpdateAccounts(IEnumerable<AccountDataModel> accounts)
    {
        ObservableCollection<SelectionViewModel> selections = new();
        foreach (AccountDataModel account in accounts)
        {
            IEnumerable<SelectionViewModel> found = AccountsSelections.Where(item => item.Data.Equals(account));
            selections.Add(found.Any() ? found.First() : new(account));
        }
        AccountsSelections = selections;
    }
}

public class SelectionViewModel : ViewModelBase
{
    private bool _isSelected = false;
    private readonly AccountDataModel _data;

    public SelectionViewModel(AccountDataModel data)
    {
        _data = data;
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => ChangeProperty(ref _isSelected, value);
    }

    public string Name
    {
        get => _data.AccountName;
    }

    public AccountDataModel Data
    {
        get => _data;
    }
}
