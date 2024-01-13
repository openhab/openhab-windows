using openHAB.Core.Client.Models;
using openHAB.Core.Services;

namespace openHAB.Core.Messages
{

    public class SitemapChanged
    {

        public SitemapChanged(OpenHABSitemap sitemap)
        {
            sitemap = sitemap;
        }

        public OpenHABSitemap Sitemap
        {
            get;
            private set;
        }
    }
}
