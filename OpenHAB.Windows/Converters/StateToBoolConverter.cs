using System;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts a boolean string value to a .NET bool.
    /// </summary>
    public class StateToBoolConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().ToLower() == "true";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "Off";
            }

            bool toggleValue = (bool)value;

            return toggleValue ? "On" : "Off";
        }
    }
}
