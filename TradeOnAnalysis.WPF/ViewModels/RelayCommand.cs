using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace TradeOnAnalysis.WPF.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly List<Action<object?>> _executeList = new();
        private readonly Func<object?, bool>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Func<object?, bool>? canExecute = null)
        {
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            : this(canExecute)
        {
            AddExecute(execute);
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            foreach (Action<object?> execute in _executeList)
                execute(parameter);
        }

        public void AddExecute(Action<object?> execute)
        {
            _executeList.Add(execute);
        }

        public void RemoveExecute(Action<object?> execute)
        {
            _executeList.Remove(execute);
        }
    }
}
