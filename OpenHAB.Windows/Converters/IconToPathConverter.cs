using System;
using System.Text.RegularExpressions;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Windows.Services;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts an OpenHAB widget icon to a full path.
    /// </summary>
    public class IconToPathConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = (ISettingsService)DIService.Instance.Services.GetService(typeof(ISettingsService));
            var serverUrl = OpenHABHttpClient.BaseUrl;

            var widget = value as OpenHABWidget;
            var state = widget.Item?.State ?? "ON";

            var regMatch = Regex.Match(state, @"\d+");
            if (regMatch.Success)
            {
                state = regMatch.Value;
            }

            return settingsService.ServerVersion == OpenHABVersion.Two ?
                $"{serverUrl}icon/{widget.Icon}?state={state}&format=png" :
                $"{serverUrl}images/{widget.Icon}.png";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
