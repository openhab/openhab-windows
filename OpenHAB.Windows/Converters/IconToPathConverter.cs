using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
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
            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.Load();
            var serverUrl = settings.IsRunningInDemoMode.Value ? Constants.Api.DemoModeUrl : settings.OpenHABUrl;

            return settingsService.ServerVersion == OpenHABVersion.Two ?
                $"{serverUrl}icon/{value}?state=UNDEF&format=png" :
                $"{serverUrl}images/{value}.png";
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
