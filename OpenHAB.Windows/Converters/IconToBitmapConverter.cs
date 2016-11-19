using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Converters
{
    public class IconToBitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().Load();
            var serverUrl = settings.IsRunningInDemoMode.Value ? Constants.Api.DemoModeUrl : settings.OpenHABUrl;

            var url = $"{serverUrl}icon/{value}?state=UNDEF&format=png";
            return new BitmapImage(new Uri(url));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
