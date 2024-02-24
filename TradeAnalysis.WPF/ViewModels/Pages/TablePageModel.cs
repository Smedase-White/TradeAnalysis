using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeAnalysis.WPF.ViewModels
{
    public class TablePageModel : ViewModelBase
    {
        private AccountSelectModel _accountSelect = new();

        private TableType _tableType = TableType.Resale;

        private TradeTableModel _tradeTable = new();

        public TablePageModel()
        {
            AccountSelect.CloseCommand.AddExecute(obj => Task.Run(() => LoadTable()));
        }

        public AccountSelectModel AccountSelect
        {
            get => _accountSelect;
            set
            {
                ChangeProperty(ref _accountSelect, value);
                _accountSelect.CloseCommand.AddExecute(obj => Task.Run(() => LoadTable()));
            }
        }

        public TableType TableType
        {
            get => _tableType;
            set
            {
                ChangeProperty(ref _tableType, value);
                LoadTable();
            }
        }

        public IEnumerable<TableType> TableTypeValues
        {
            get => Enum.GetValues(typeof(TableType)).Cast<TableType>();
        }

        public TradeTableModel TradeTable
        {
            get => _tradeTable;
            set => ChangeProperty(ref _tradeTable, value);
        }

        public void LoadTable()
        {
            TradeTable.LoadTable(AccountSelect.SelectedAccounts, TableType);
        }
    }
}
