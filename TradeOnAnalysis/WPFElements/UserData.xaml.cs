using System;
using System.Windows.Controls;

namespace TradeOnAnalysis.Assets.WPFElements
{
    /// <summary>
    /// Логика взаимодействия для UserData.xaml
    /// </summary>
    public partial class UserData : UserControl
    {
        public UserData()
        {
            InitializeComponent();
            EndDatePicker.Text = DateTime.Now.ToShortDateString();
        }

        public string MarketApi => ApiBox.Text;

        public DateTime StartDate => StartDatePicker.SelectedDate ?? DateTime.Now.AddDays(-30);

        public DateTime EndDate => EndDatePicker.SelectedDate ?? DateTime.Now;
    }
}
