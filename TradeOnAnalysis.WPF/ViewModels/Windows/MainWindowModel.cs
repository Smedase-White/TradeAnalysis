using System.Windows;

namespace TradeOnAnalysis.WPF.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        private readonly AccountsPageModel _accountPage = new();
        private readonly TablePageModel _tablePage = new();
        private readonly ChartsPageModel _chartsPage = new();

        private int _selectedIndex = 0;

        private AccountSelectModel _accountSelect = new();

        private RelayCommand? _exitCommand;

        public MainWindowModel()
        {
            TablePage.AccountSelect = _accountSelect;
            ChartsPage.AccountSelect = _accountSelect;
        }

        public AccountsPageModel AccountsPage
        {
            get => _accountPage;
        }

        public TablePageModel TablePage
        {
            get => _tablePage;
        }

        public ChartsPageModel ChartsPage
        {
            get => _chartsPage;
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                ChangePage();
                ChangeProperty(ref _selectedIndex, value);
            }
        }

        private void ChangePage()
        {
            if (SelectedIndex == 0)
                _accountSelect.UpdateAccounts(AccountsPage.LoadedAccounts);
        }

        public RelayCommand ExitCommand
        {
            get => _exitCommand ??= new(obj => { Exit(obj as Window); });
            set => ChangeProperty(ref _exitCommand, value);
        }

        public void Exit(Window window)
        {
            window.Close();
        }
    }
}
