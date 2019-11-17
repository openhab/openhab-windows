using System;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection configuration.
    /// </summary>
    /// <seealso cref="OpenHAB.Core.Model.ObservableBase" />
    public class ConnectionConfigViewModel : ObservableBase
    {
        private readonly OpenHABConnection _connectionConfig;
        private string _url;
        private string _password;
        private string _username;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionConfigViewModel"/> class.
        /// </summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        public ConnectionConfigViewModel(OpenHABConnection connectionConfig)
        {
            _connectionConfig = connectionConfig;

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
                if (_url == value)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                {
                    _url = value + "/";
                }
                else
                {
                    _url = value;
                }

                SetProperty(ref _url, value);
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
    }
}
