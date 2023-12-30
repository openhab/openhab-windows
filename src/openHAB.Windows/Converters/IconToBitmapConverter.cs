using System;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using Mapsui.Widgets;
using Microsoft.SqlServer.Server;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;

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
            ISettingsService settingsService = (ISettingsService)DIService.Instance.GetService<ISettingsService>();
            OpenHABVersion openHABVersion = settingsService.ServerVersion;
            Settings settings = settingsService.Load();

            OpenHABWidget widget = value as OpenHABWidget;
            string state = widget.Item?.State ?? "ON";

            var serverUrl = Core.Common.OpenHABHttpClient.BaseUrl;
            string iconFormat = settings.UseSVGIcons ? "svg" : "png";
            string url = openHABVersion == OpenHABVersion.Two || openHABVersion == OpenHABVersion.Three || openHABVersion == OpenHABVersion.Four ?
                $"{serverUrl}icon/{widget.Icon}?state={state}&format={iconFormat}" :
                $"{serverUrl}images/{widget.Icon}.png";

            IIconCaching iconCaching = DIService.Instance.GetService<IIconCaching>();
            var iconPathTask = iconCaching.ResolveIconPath(url, iconFormat).ConfigureAwait(false);
            string iconPath = iconPathTask.GetAwaiter().GetResult();

            if (settings.UseSVGIcons)
            {
                return new SvgImageSource(new Uri(iconPath));
            }
            else
            {
                return new BitmapImage(new Uri(iconPath));
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
