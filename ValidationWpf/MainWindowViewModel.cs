using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ValidationWpf
{
    internal class MainWindowViewModel : NotifyPropertyChanged
    {
        public ICommand ResetCommand { get; private set; }

        private string _filePath;

        public string FilePath
        {
            get => _filePath;
            set
            {
                var r = new Regex(@"[\w\s\\.:\-!~]");
                if (r.Matches(value).Count == value.Length)
                {
                    SetProperty(ref _filePath, value);
                    return;
                }

                throw new ArgumentException("Not an valid windows path");
            }
        }

        public MainWindowViewModel()
        {
            ResetCommand = new DelegateCommand(_ => Reset());
        }

        private void Reset()
        {
            FilePath = @"C:\Temp";
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action<object> _execute;

        public DelegateCommand(Action<object> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute?.Invoke(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}