using openHAB.Core.Client.Common;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Contracts
{
    /// <summary>
    /// The main SDK interface to OpenHAB.
    /// </summary>
    public interface IOpenHABClient
    {
        /// <summary>
        /// Gets information about the openHAB server.
        /// </summary>
        /// <returns>Server information about openHAB instance.</returns>
        Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo();

        /// <summary>
        /// Gets the openHAB item by name from Server.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns>openHab item object. </returns>
        Task<OpenHABItem> GetItemByName(string itemName);

        /// <summary>
        /// Loads all the sitemaps.
        /// </summary>
        /// <param name="version">The version of OpenHAB running on the server.</param>
        /// <param name="filters">Filters for sitemap list.</param>
        /// <returns>A list of sitemaps.</returns>
        Task<ICollection<OpenHABSitemap>> LoadSitemaps(OpenHABVersion version, List<Func<OpenHABSitemap, bool>> filters);

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
        /// <returns>Operation result if the command was successful or not.</returns>
        Task<HttpResponseResult<bool>> SendCommand(OpenHABItem item, string command);

        /// <summary>
        /// Reset the connection to the OpenHAB server after changing the settings in the App.
        /// </summary>
        /// <param name="localConnection">The local connection to reset.</param>
        /// <param name="remoteConnection">The remote connection to reset.</param>
        /// <param name="isRunningInDemoMode">Indicates whether the App is running in demo mode.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> ResetConnection(Connection.Models.Connection localConnection, Connection.Models.Connection remoteConnection, bool? isRunningInDemoMode);

        /// <summary>
        /// Starts listening to server events.
        /// </summary>
        void StartItemUpdates(System.Threading.CancellationToken token);
    }
}
