using System;
using openHAB.Core.Model;
using openHAB.Windows.Services;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Services.Contracts;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts an icon path to a bitmap object.
    /// </summary>
    public class IconToBitmapConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = (ISettingsService)DIService.Instance.GetService<ISettingsService>();
            OpenHABVersion openHABVersion = settingsService.ServerVersion;

            var serverUrl = Core.Common.OpenHABHttpClient.BaseUrl;
            string url = openHABVersion == OpenHABVersion.Two || openHABVersion == OpenHABVersion.Three ? $"{serverUrl}icon/{value}?state=UNDEF&format=png" : $"{serverUrl}images/{value}.png";

            return new BitmapImage(new Uri(url));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
