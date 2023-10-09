using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LogToGrafity
{
    public class CollectionSizeConverter : MarkupExtension, IValueConverter
    {
        public object IfEmpty { get; set; } = Binding.DoNothing;
        public object IfNotEmpty { get; set; } = Binding.DoNothing;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                return collection.Count == 0 ? IfEmpty : IfNotEmpty;
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
