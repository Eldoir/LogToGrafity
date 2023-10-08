using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LogToGrafity
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color col)
            {
                // convert it to right class first
                value = Color.FromArgb(col.A, col.R, col.G, col.B);
            }

            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
