using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace LogToGrafity
{
    public class DragNDropStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DragNDropState state)
            {
                return state switch
                {
                    DragNDropState.Idle => Color.Transparent,
                    DragNDropState.Accept => Color.LawnGreen,
                    DragNDropState.Reject => Color.IndianRed,
                    _ => Color.Transparent
                };
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
