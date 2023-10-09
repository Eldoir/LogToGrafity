using System;
using System.Windows.Input;

namespace LogToGrafity
{
    public class DelegateCommand : ICommand
    {
        public DelegateCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand
        
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke();
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
    }

    public class DelegateCommand<T> : ICommand
    {
        public DelegateCommand(Action<T> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T value)
            {
                _execute?.Invoke(value);
            }
        }

        #endregion

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly Action<T> _execute;
        private readonly Func<bool>? _canExecute;
    }
}
