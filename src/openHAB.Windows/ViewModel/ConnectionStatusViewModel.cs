using System;
using System.Threading.Tasks;
using OpenHAB.Core.Common;
using OpenHAB.Core.Model;
using OpenHAB.Core.Model.Connection;
using OpenHAB.Core.SDK;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection status.
    /// </summary>
    public class ConnectionStatusViewModel : ViewModelBase<object>
    {
        private readonly IOpenHAB _openHabsdk;

        private OpenHABUrlState _connectionState;

        private string _runtimeVersion;
        private string _build;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStatusViewModel"/> class.
        /// </summary>
        /// <param name="openHabsdk">OpenHABSDK class.</param>
        public ConnectionStatusViewModel(IOpenHAB openHabsdk)
            : base(null)
        {
            _openHabsdk = openHabsdk;
            _connectionState = OpenHABUrlState.Unknown;
        }

        /// <summary>
        /// Gets or sets the state for OpenHab connection.
        /// </summary>
        /// <value>The state of the connection.</value>
        public OpenHABUrlState State
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
        public void CheckConnectionSettings(string url, OpenHABConnection connection)
        {
            Task<HttpResponseResult<ServerInfo>> result = _openHabsdk.GetOpenHABServerInfo(connection);
            result.ContinueWith(async (task) =>
            {
                OpenHABUrlState urlState = OpenHABUrlState.Unknown;
                string runtimeVersion = string.Empty;
                string build = string.Empty;

                if (task.IsCompletedSuccessfully && task.Result.Content != null)
                {
                    ServerInfo serverInfo = task.Result.Content;

                    runtimeVersion = serverInfo.RuntimeVersion;
                    build = serverInfo.Build;
                    urlState = OpenHABUrlState.OK;
                }
                else
                {
                    urlState = OpenHABUrlState.Failed;
                }

                CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    State = urlState;
                    RuntimeVersion = runtimeVersion;
                    Build = build;
                });
            });
        }
    }
}
