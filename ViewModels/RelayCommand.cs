using CinemaManagement.Models;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace CinemaManagement.ViewModels
{
    public class RelayCommand : ICommand, INotifyPropertyChanged
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private (bool, Account, string) value;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand((bool, Account, string) value)
        {
            this.value = value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int SubscriberCount => CanExecuteChanged?.GetInvocationList().Length ?? 0;
    }
}
