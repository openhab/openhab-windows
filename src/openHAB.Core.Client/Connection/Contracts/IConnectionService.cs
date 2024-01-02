using openHAB.Core.Client.Common;
using openHAB.Core.Client.Models;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Connection.Contracts
{
    /// <summary>
    /// Service for connection management.
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Gets or sets the current connection to openHAB instance.
        /// </summary>
        Models.Connection CurrentConnection
        {
            get;
        }

        /// <summary>Checks the URL reachability.</summary>
        /// <param name="connection">Defines settings for local or remote connections.</param>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<HttpResponseResult<bool>> CheckUrlReachability(Models.Connection connection);

        /// <summary>Detects if connection is local or remote and provides the connection information.</summary>
        /// <param name="settings">The app settings.</param>
        /// <returns>Return the connection information for local/remote.</returns>
        Task<Models.Connection> DetectAndRetriveConnection(Models.Connection localConnection, Models.Connection remoteConnection, bool isRunningInDemoMode);

        /// <summary>
        /// Gets information about the openHAB server.
        /// </summary>
        /// <param name="connection">Connection information.</param>
        /// <returns>Server information about openHAB instance.</returns>
        Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo(Models.Connection connection);
    }
}