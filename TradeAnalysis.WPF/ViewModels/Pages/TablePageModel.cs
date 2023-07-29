namespace TradeAnalysis.WPF.ViewModels
{
    public class TablePageModel : ViewModelBase
    {
        private AccountSelectModel _accountSelect = new();
        private TradeTableModel _tradeTable = new();

        public TablePageModel()
        {
            AccountSelect.CloseCommand.AddExecute(obj => { LoadTable(); });
        }

        public AccountSelectModel AccountSelect
        {
            get => _accountSelect;
            set
            {
                ChangeProperty(ref _accountSelect, value);
                _accountSelect.CloseCommand.AddExecute(obj => { LoadTable(); });
            }
        }

        public TradeTableModel TradeTable
        {
            get => _tradeTable;
            set => ChangeProperty(ref _tradeTable, value);
        }

        public void LoadTable()
        {
            TradeTable.LoadTable(AccountSelect.SelectedAccounts);
        }
    }
}
