using System.Threading.Tasks;
using openHAB.Core.Connection;
using openHAB.Core.Common;
using openHAB.Core.Model;

namespace openHAB.Core.Services.Contracts
{
    /// <summary>
    /// Service for connection management.
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Gets information about the openHAB server.
        /// </summary>
        /// <param name="connection">Connection information.</param>
        /// <returns>Server information about openHAB instance.</returns>
        Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo(OpenHABConnection connection);

        /// <summary>Checks the URL reachability.</summary>
        /// <param name="connection">Defines settings for local or remote connections.</param>
        /// <returns>>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<HttpResponseResult<bool>> CheckUrlReachability(OpenHABConnection connection);

        /// <summary>Detects if connection is local or remote and provides the connection information.</summary>
        /// <param name="settings">The app settings.</param>
        /// <returns>Return the connection information for local/remote.</returns>
        Task<OpenHABConnection> DetectAndRetriveConnection(Settings settings);
    }
}