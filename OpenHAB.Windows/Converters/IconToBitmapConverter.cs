using System;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Windows.Services;
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
            ISettingsService settingsService = (ISettingsService)DIService.Instance.Services.GetService(typeof(ISettingsService));
            Settings settings = settingsService.Load();

            var serverUrl = Core.Common.OpenHABHttpClient.BaseUrl;

            string iconFormat = settings.UseSVGIcons ? "svg" : "png";
            string url = settingsService.ServerVersion == OpenHABVersion.Two ? $"{serverUrl}icon/{value}?state=UNDEF&format={iconFormat}" : $"{serverUrl}images/{value}.{iconFormat}";

            return new BitmapImage(new Uri(url));
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
