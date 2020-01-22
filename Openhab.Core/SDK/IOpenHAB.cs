using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenHAB.Core.Common;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.SDK
{
    /// <summary>
    /// The main SDK interface to OpenHAB.
    /// </summary>
    public interface IOpenHAB
    {
        /// <summary>
        /// Is the server running OpenHAB 1 or OpenHAB 2?.
        /// </summary>
        /// <returns>Server main version of OpenHAB.</returns>
        Task<OpenHABVersion> GetOpenHABVersion();

        /// <summary>
        /// Loads all the sitemaps.
        /// </summary>
        /// <param name="version">The version of OpenHAB running on the server.</param>
        /// <param name="filters">Filters for sitemap list.</param>
        /// <returns>A list of sitemaps.</returns>
        Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version, List<Func<OpenHABSitemap, bool>> filters);

        /// <summary>
        /// Loads all the items in a sitemap.
        /// </summary>
        /// <param name="sitemap">The sitemap to load the items from.</param>
        /// <param name="version">The version of OpenHAB running on the server.</param>
        /// <returns>A list of items in the selected sitemap.</returns>
        Task<ICollection<OpenHABWidget>> LoadItemsFromSitemap(OpenHABSitemap sitemap, OpenHABVersion version);

        /// <summary>
        /// Sends a command to an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="command">The Command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendCommand(OpenHABItem item, string command);

        /// <summary>
        /// Reset the connection to the OpenHAB server after changing the settings in the app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> ResetConnection();

        /// <summary>
        /// Starts listening to server events.
        /// </summary>
        void StartItemUpdates();

        /// <summary>Checks the URL reachability.</summary>
        /// <param name="connection">Defines settings for local or remote connections.</param>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> CheckUrlReachability(OpenHABConnection connection);
    }
}
