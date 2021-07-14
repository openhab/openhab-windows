using System;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Data;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// Converts objects to sitemaps, for use in compiled bindings.
    /// </summary>
    public class ObjectToSitemapConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value as OpenHABSitemap;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as OpenHABSitemap;
        }
    }
}
