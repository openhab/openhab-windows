using System.Collections.Generic;
using System.Threading.Tasks;
using Openhab.Model;

namespace Openhab.Core.SDK
{
    public interface IOpenHAB
    {
        /// <summary>
        /// Is the server running OpenHAB 1 or OpenHAB 2?
        /// </summary>
        /// <returns>Server main version of OpenHAB</returns>
        Task<OpenHABVersion> GetOpenHABVersion();

        /// <summary>
        /// Loads all the sitemaps
        /// </summary>
        /// <param name="version">The version of OpenHAB running on the server</param>
        /// <returns>A list of sitemaps</returns>
        Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version);

        /// <summary>
        /// Loads all the items in a sitemap
        /// </summary>
        /// <param name="sitemap">The sitemap to load the items from</param>
        /// <param name="version">The version of OpenHAB running on the server</param>
        /// <returns>A list of items in the selected sitemap</returns>
        Task<ICollection<OpenHABWidget>> LoadItemsFromSitemap(OpenHABSitemap sitemap, OpenHABVersion version);

        /// <summary>
        /// Sends a command to an item
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="command">The Command</param>
        /// <returns></returns>
        Task SendCommand(OpenHABItem item, string command);
    }
}
