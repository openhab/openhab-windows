using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using openHAB.Core.Client.Models;

namespace openHAB.Windows.Converters
{
    /// <summary>Template selector to differentiate between sitemaps and general menu items.</summary>
    [ContentProperty(Name = "SiteMapMenuItemTemplate")]
    public class MenuItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>Gets or sets the sitemap menu item template.</summary>
        /// <value>The site map menu item template.</value>
        public DataTemplate SiteMapMenuItemTemplate
        {
            get;
            set;
        }

        /// <summary>Gets or sets the item template for default menu items.</summary>
        /// <value>The default item template.</value>
        public DataTemplate DefaultItemTemlate
        {
            get; set;
        }

        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            Sitemap sitemapMenuItem = item as Sitemap;

            if (sitemapMenuItem != null)
            {
                return SiteMapMenuItemTemplate;
            }

            return DefaultItemTemlate;
        }
    }
}
