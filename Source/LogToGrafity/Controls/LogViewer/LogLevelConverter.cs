using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LogToGrafity
{
    public sealed class LogLevelConverter : MarkupExtension, IValueConverter
    {
        public object Info { get; set; } = Binding.DoNothing;
        public object Warning { get; set; } = Binding.DoNothing;
        public object Error { get; set; } = Binding.DoNothing;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogLevel logLevel)
            {
                return logLevel switch
                {
                    LogLevel.Info => Info,
                    LogLevel.Warning => Warning,
                    LogLevel.Error => Error,
                    _ => throw new NotImplementedException()
                };
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
