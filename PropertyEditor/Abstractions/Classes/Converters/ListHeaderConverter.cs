using System;

namespace VisualPropertyEditor.Abstractions.Classes.Converters
{
    internal class ListHeaderConverter : BaseConverter, System.Windows.Data.IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }


}
