using System;

namespace VisualPropertyEditor.Abstractions.Classes.Converters
{
    internal class MarginConverter : BaseConverter, System.Windows.Data.IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return new System.Windows.Thickness(System.Convert.ToDouble(value) * 40, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
