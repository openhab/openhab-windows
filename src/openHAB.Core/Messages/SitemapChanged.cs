using openHAB.Core.Client.Models;
using openHAB.Core.Services;

namespace openHAB.Core.Messages
{

    public class SitemapChanged
    {

        public SitemapChanged(Sitemap sitemap)
        {
            sitemap = sitemap;
        }

        public Sitemap Sitemap
        {
            get;
            private set;
        }
    }
}
