using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TradeAnalysis.Core.Utils.MarketItems;

namespace TradeAnalysis.WPF.ViewModels;

public class TradeTableModel : ViewModelBase
{
    private ObservableCollection<TableElement> _tableElements = new();

    public TradeTableModel()
    {

    }

    public ObservableCollection<TableElement> TableElements
    {
        get => _tableElements;
        set => ChangeProperty(ref _tableElements, value);
    }

    public void LoadTable(IEnumerable<AccountDataModel> accounts, TableType type)
    {
        List<MarketItem> totalHistory = new();
        foreach (AccountDataModel account in accounts)
            totalHistory.AddRange(type switch
            {
                TableType.Resale => account.Account!.TradeHistory!,
                TableType.Deposit => account.Account!.DepositItemsHistory!,
                TableType.Unsold => account.Account!.UnsoldItemsHistory!,
                _ => Array.Empty<MarketItem>()
            });
        TableElements = new(from trade in totalHistory
                            orderby trade.BuyInfo?.Time ?? trade.SellInfo!.Time
                            select new TableElement(trade));
    }
}

public readonly struct TableElement
{
    public string Name { get; init; }
    public DateOnly BuyDate { get; init; }
    public double BuyPrice { get; init; }
    public DateOnly SellDate { get; init; }
    public double SellPrice { get; init; }

    public TableElement(MarketItem item)
    {
        Name = item.Name;
        if (item.BuyInfo is not null)
        {
            BuyDate = DateOnly.FromDateTime(item.BuyInfo.Time);
            BuyPrice = -Math.Round(item.BuyInfo.Amount, 2);
        }
        if (item.SellInfo is not null)
        {
            SellDate = DateOnly.FromDateTime(item.SellInfo.Time);
            SellPrice = Math.Round(item.SellInfo.Amount, 2);
        }
    }
}

public enum TableType
{
    Resale,
    Deposit,
    Unsold
}
