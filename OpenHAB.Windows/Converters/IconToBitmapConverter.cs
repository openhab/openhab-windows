using System;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts an icon path to a bitmap object.
    /// </summary>
    public class IconToBitmapConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = (ISettingsService)App.Container.Services.GetService(typeof(ISettingsService));
            var serverUrl = Core.Common.OpenHABHttpClient.BaseUrl;

            string url = settingsService.ServerVersion == OpenHABVersion.Two ? $"{serverUrl}icon/{value}?state=UNDEF&format=png" : $"{serverUrl}images/{value}.png";

            return new BitmapImage(new Uri(url));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
