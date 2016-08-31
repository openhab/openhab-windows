using System;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    public class StateToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString().ToLower() == "true";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "Off";
            }

            bool toggleValue = (bool) value;

            return toggleValue ? "On" : "Off";
        }
    }
}
