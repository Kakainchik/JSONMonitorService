using System;
using System.Windows.Input;

namespace WPFApplication.Core
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        private Action<object?> execute;
        private Func<object?, bool>? canExecute;

        #region ICommand implementation

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }

        #endregion
    }
}