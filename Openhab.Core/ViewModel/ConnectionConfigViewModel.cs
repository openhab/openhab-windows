using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection configuration.
    /// </summary>
    /// <seealso cref="OpenHAB.Core.Model.ObservableBase" />
    public class ConnectionConfigViewModel : ObservableBase
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

                SetProperty(ref _url, tempUrl);
                _connectionConfig.Url = _url;
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
                SetProperty(ref _username, value);
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
                SetProperty(ref _password, value);
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
            set => SetProperty(ref _urlState, value);
        }

        private async void CheckLocalUrl(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            UrlState = OpenHABUrlState.Unknown;
            if (await _openHabsdk.CheckUrlReachability(url, Common.OpenHABHttpClientType.Local).ConfigureAwait(false))
            {
                UrlState = OpenHABUrlState.OK;
            }
            else
            {
                UrlState = OpenHABUrlState.Failed;
            }
        }
    }
}
