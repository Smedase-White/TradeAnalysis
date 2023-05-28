using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace TradeOnAnalysis.Assets.WPFElements
{
    /// <summary>
    /// Логика взаимодействия для ItemGrid.xaml
    /// </summary>
    public partial class ItemsGrid : UserControl
    {
        public ItemsGrid()
        {
            InitializeComponent();
        }

        private void Display(IEnumerable<GridElement> elements)
        {
            Grid.ItemsSource = elements;
            var columns = Grid.Columns;
            columns[0].Width = 420;
            columns[1].Width = 90;
            columns[2].Width = 90;
            columns[3].Width = 90;
            columns[4].Width = 60;
        }

        public void Display(IEnumerable<Item> items, Func<Item, bool> predicate)
        {
            var elements = from item in items
                           where predicate(item)
                           select new GridElement(item);
            Display(elements);
        }

        public void DisplayAll(IEnumerable<Item> items)
        {
            var elements = from item in items
                           select new GridElement(item);
            Display(elements);
        }
    }

    public readonly struct GridElement
    {
        public string Name { get; init; }
        public DateOnly? BuyDate { get; init; }
        public double? BuyPrice { get; init; }
        public DateOnly? SellDate { get; init; }
        public double? SellPrice { get; init; }

        public GridElement(Item item)
        {
            Name = item.Name;
            if (item.BuyInfo != null)
            {
                BuyDate = DateOnly.FromDateTime(item.BuyInfo!.Date);
                BuyPrice = item.BuyInfo!.Price;
            }
            if (item.SellInfo != null)
            {
                SellDate = DateOnly.FromDateTime(item.SellInfo!.Date);
                SellPrice = item.SellInfo!.Price;
            }
        }
    }
}
