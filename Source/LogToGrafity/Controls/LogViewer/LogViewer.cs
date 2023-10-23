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
    }
}
