using System;
using Microsoft.UI.Xaml.Data;
using openHAB.Core.Model;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts a boolean string value to a .NET bool.
    /// </summary>
    public class StateToBoolConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }

            return string.Compare(value.ToString(), OpenHABCommands.OnCommand, true) == 0;
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
