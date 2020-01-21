using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using OpenHAB.Core.Common;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection configuration.
    /// </summary>
    public class ConnectionConfigViewModel : ViewModelBase<OpenHABConnection>
    {
        private readonly OpenHABConnection _connectionConfig;
        private readonly IOpenHAB _openHabsdk;
        private string _url;
        private string _password;
        private string _username;
        private ICommand _urlCheckCommand;
        private OpenHABUrlState _urlState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionConfigViewModel"/> class.
        /// </summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        /// <param name="openHabsdk">OpenHABSDK class</param>
        public ConnectionConfigViewModel(OpenHABConnection connectionConfig, IOpenHAB openHabsdk)
            : base(connectionConfig)
        {
            _connectionConfig = connectionConfig;
            _openHabsdk = openHabsdk;

            _url = _connectionConfig.Url;
            _username = connectionConfig.Username;
            _password = connectionConfig.Password;
        }

        /// <summary>
        /// Gets or sets the url to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get
            {
                return _url;
            }

            set
            {
                string tempUrl = string.Empty;
                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                {
                    tempUrl = value + "/";
                }
                else
                {
                    tempUrl = value;
                }

                Set(ref _url, tempUrl);
                _connectionConfig.Url = _url;

                OnPropertyChanged(nameof(Subtitle));
            }
        }

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        /// <value>The subtitle.</value>
        public string Subtitle
        {
            get
            {
                if (string.IsNullOrEmpty(_url))
                {
                    return "Not configured";
                }

                if (Uri.TryCreate(_url, UriKind.Absolute, out Uri uri) && 
                    string.Compare(uri.Scheme.ToUpperInvariant(), "HTTPS", StringComparison.InvariantCulture) == 0)
                {
                    return "Conntected to " + _url;
                }

                return "Unsecure connected to" + _url;
            }
        }

        /// <summary>
        /// Gets or sets the username for the local OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                Set(ref _username, value);
                _connectionConfig.Username = value;
            }
        }

        /// <summary>
        /// Gets or sets the password for the local OpenHAB connection.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                Set(ref _password, value);
                _connectionConfig.Password = value;
            }
        }

        /// <summary>
        /// Gets the command for local url check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand UrlCheckCommand => _urlCheckCommand ?? (_urlCheckCommand = new RelayCommand<object>(CheckLocalUrl));

        /// <summary>
        /// Gets or sets the state for OpenHab local url.
        /// </summary>
        /// <value>The state of the local URL.</value>
        public OpenHABUrlState UrlState
        {
            get => _urlState;
            set => Set(ref _urlState, value);
        }

        private async void CheckLocalUrl(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            OpenHABUrlState urlState = OpenHABUrlState.Unknown;
            if (await _openHabsdk.CheckUrlReachability(url, OpenHABHttpClientType.Local).ConfigureAwait(false))
            {
                urlState = OpenHABUrlState.OK;
            }
            else
            {
                urlState = OpenHABUrlState.Failed;
            }

            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                UrlState = urlState;
            });
        }
    }
}
