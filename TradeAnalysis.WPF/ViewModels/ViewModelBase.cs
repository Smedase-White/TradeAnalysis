using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TradeAnalysis.WPF.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool ChangeProperty<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            if (property?.Equals(value) == true)
                return false;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected void OnChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
