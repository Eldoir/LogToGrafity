using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LogToGrafity
{
    public class LogViewer : UserControl, INotifyPropertyChanged
    {
        public LogViewer()
        {
            ToggleFilterCommand = new DelegateCommand<LogLevel>(ToggleFilter);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        public static DependencyProperty LogsProperty = DependencyProperty.Register(
            nameof(Logs),
            typeof(ObservableCollection<LogElement>),
            typeof(LogViewer),
            new PropertyMetadata(new ObservableCollection<LogElement>()));

        public ObservableCollection<LogElement> Logs
        {
            get => (ObservableCollection<LogElement>)GetValue(LogsProperty);
            set => SetValue(LogsProperty, value);
        }

        public ICommand ToggleFilterCommand { get; }

        public IEnumerable<LogElement> FilteredLogs => Logs.Where(l => _filters.Contains(l.Level));

        public void ToggleFilter(LogLevel filter)
        {
            if (_filters.Contains(filter))
            {
                _filters.Remove(filter);
            }
            else
            {
                _filters.Add(filter);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilteredLogs)));
        }

        public void LogInfo(string message)
        {
            Logs.Add(new LogElement(LogLevel.Info, message));
        }

        public void LogWarning(string message)
        {
            Logs.Add(new LogElement(LogLevel.Warning, message));
        }

        public void LogError(string message)
        {
            Logs.Add(new LogElement(LogLevel.Error, message));
        }

        private readonly List<LogLevel> _filters = new() { LogLevel.Info, LogLevel.Warning, LogLevel.Error };
    }
}
