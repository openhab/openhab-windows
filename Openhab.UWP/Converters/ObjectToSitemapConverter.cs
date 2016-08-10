using System;
using Windows.UI.Xaml.Data;
using Openhab.Model;

namespace Openhab.UWP.Converters
{
    public class ObjectToSitemapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value as OpenHABSitemap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as OpenHABSitemap;
        }
    }
}
