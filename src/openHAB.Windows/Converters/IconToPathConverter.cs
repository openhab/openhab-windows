using Microsoft.UI.Xaml.Data;
using openHAB.Core.Common;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using System;
using System.Text.RegularExpressions;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts an OpenHAB widget icon to a full path.
    /// </summary>
    public class IconToPathConverter : IValueConverter
    {
        private Settings _settings;

        /// <summary>Initializes a new instance of the <see cref="IconToPathConverter" /> class.</summary>
        public IconToPathConverter()
        {
            _settings = DIService.Instance.GetService<Settings>();
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = (ISettingsService)DIService.Instance.GetService<ISettingsService>();
            OpenHABVersion openHABVersion = settingsService.ServerVersion;

            var serverUrl = OpenHABHttpClient.BaseUrl;

            var widget = value as OpenHABWidget;
            var state = widget.Item?.State ?? "ON";
            string iconFormat = _settings.UseSVGIcons ? "svg" : "png";

            var regMatch = Regex.Match(state, @"\d+");
            if (regMatch.Success)
            {
                state = regMatch.Value;
            }

            return openHABVersion == OpenHABVersion.Two || openHABVersion == OpenHABVersion.Three ?
                $"{serverUrl}icon/{widget.Icon}?state={state}&format={iconFormat}" :
                $"{serverUrl}images/{widget.Icon}.png";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
