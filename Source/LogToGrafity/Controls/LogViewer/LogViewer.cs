using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace LogToGrafity
{
    public class LogViewer : UserControl
    {
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
    }
}
