using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Contracts.Services;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts an OpenHAB widget icon to a full path
    /// </summary>
    public class IconToPathConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string serverUrl = ServiceLocator.Current.GetInstance<ISettingsService>().Load().OpenHABUrl;

            return $"{serverUrl}/images/{value}.png";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
