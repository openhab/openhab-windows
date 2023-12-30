using System;
using Microsoft.UI.Xaml.Data;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts a string to a double, returning 0 if the string is invalid.
    /// </summary>
    public class StringToDoubleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string valueString = value.ToString();
            double valueDouble;

            double.TryParse(valueString, out valueDouble);
            return valueDouble;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
