using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TradeAnalysis.Core.Utils;

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

    public void LoadTable(IEnumerable<AccountDataModel> accounts)
    {
        List<Item> totalHistory = new();
        foreach (AccountDataModel account in accounts)
            totalHistory.AddRange(account.Statistics!.TradeHistory!);
        TableElements = new(from trade in totalHistory
                            orderby trade.BuyInfo!.Date
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

    public TableElement(Item item)
    {
        Name = item.Name;
        BuyDate = DateOnly.FromDateTime(item.BuyInfo!.Date);
        BuyPrice = item.BuyInfo!.Price;
        SellDate = DateOnly.FromDateTime(item.SellInfo!.Date);
        SellPrice = item.SellInfo!.Price;
    }
}
