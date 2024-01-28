using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using openHAB.Core.Client.Common;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using ConnectionState = openHAB.Core.Client.Connection.Models.ConnectionState;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection status.
    /// </summary>
    public class ConnectionStatusViewModel : ViewModelBase<object>
    {
        private readonly IConnectionService _connectionService;
        private ConnectionState _connectionState;
        private string _runtimeVersion;
        private string _build;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStatusViewModel"/> class.
        /// </summary>
        /// <param name="connectionService">The connection service.</param>
        public ConnectionStatusViewModel(IConnectionService connectionService)
            : base(null)
        {
            _connectionState = ConnectionState.Unknown;
            _connectionService = connectionService;
        }

        /// <summary>
        /// Gets or sets the state for OpenHab connection.
        /// </summary>
        /// <value>The state of the connection.</value>
        public ConnectionState State
        {
            get => _connectionState;
            set => Set(ref _connectionState, value);
        }

        /// <summary>Gets or sets the runtime version.</summary>
        /// <value>The runtime version.</value>
        public string RuntimeVersion
        {
            get => _runtimeVersion;
            set => Set(ref _runtimeVersion, value, true);
        }

        /// <summary>Gets or sets the build.</summary>
        /// <value>The build.</value>
        public string Build
        {
            get => _build;
            set => Set(ref _build, value, true);
        }

        /// <summary>Checks the connection settings.</summary>
        /// <param name="url">The URL.</param>
        /// <param name="connection">The connection.</param>
        public void CheckConnectionSettings(string url, Connection connection)
        {
            Task<HttpResponseResult<ServerInfo>> result = _connectionService.GetOpenHABServerInfo(connection);
            result.ContinueWith(async (task) =>
            {
                ConnectionState connectionState = ConnectionState.Unknown;
                string runtimeVersion = string.Empty;
                string build = string.Empty;

                if (task.IsCompletedSuccessfully && task.Result.Content != null)
                {
                    ServerInfo serverInfo = task.Result.Content;

                    runtimeVersion = serverInfo.RuntimeVersion;
                    build = serverInfo.Build;
                    connectionState = ConnectionState.OK;
                }
                else
                {
                    connectionState = ConnectionState.Failed;
                }

                DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                await App.DispatcherQueue.EnqueueAsync(() =>
                {
                    State = connectionState;
                    RuntimeVersion = runtimeVersion;
                    Build = build;
                });

                StrongReferenceMessenger.Default.Send<ConnectionStatusChanged>(new ConnectionStatusChanged(connectionState));
            });
        }
    }
}
