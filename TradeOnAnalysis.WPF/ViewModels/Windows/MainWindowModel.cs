namespace TradeOnAnalysis.WPF.ViewModels
{
    public class MainWindowModel : ViewModelBase
    {
        private readonly AccountsPageModel _accountPage = new();
        private readonly TablePageModel _tablePage = new();
        private readonly ChartsPageModel _chartsPage = new();

        private int _selectedIndex = 0;

        private AccountSelectModel _accountSelect = new();

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
    }
}
