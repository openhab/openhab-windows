using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Converters
{
    public class IconToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.Load();
            var serverUrl = settings.IsRunningInDemoMode.Value ? Constants.Api.DemoModeUrl : settings.OpenHABUrl;

            string url = string.Empty;
            url = settingsService.ServerVersion == OpenHABVersion.Two ?
                $"{serverUrl}icon/{value}?state=UNDEF&format=png" :
                $"{serverUrl}/images/{value}.png";

            return new BitmapImage(new Uri(url));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
