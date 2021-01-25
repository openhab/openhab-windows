using OpenHAB.Windows.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace OpenHAB.Windows.Converters
{
    [ContentProperty(Name = "SiteMapMenuItemTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SiteMapMenuItemTemplate
        {
            get;
            set;
        }

        public DataTemplate DefaultItemTemlate
        {
            get; set;
        }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            SitemapViewModel sitemapMenuItem = item as SitemapViewModel;

            if (sitemapMenuItem != null)
            {
                return SiteMapMenuItemTemplate;
            }

            return DefaultItemTemlate;
        }
    }
}
