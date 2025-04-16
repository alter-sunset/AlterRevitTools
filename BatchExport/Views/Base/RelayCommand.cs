using System;
using System.Windows.Input;

namespace AlterTools.BatchExport.Views.Base
{
    public class RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) : ICommand
    {
        private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<object, bool> _canExecute = canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute is null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}