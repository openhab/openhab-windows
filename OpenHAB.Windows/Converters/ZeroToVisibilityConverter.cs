using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts zero to visibility type (0 = Visibility.Visible, else Visibility.Collapsed).
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class ZeroToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            int numberOfElements = (int)value;
            return numberOfElements == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
