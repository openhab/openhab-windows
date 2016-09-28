namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Class that holds all the OpenHAB Windows app settings
    /// </summary>
    public class Settings : ObservableBase
    {
        private bool? _isRunningInDemoMode;
        private string _openHABUrl;
        private string _openHABRemoteUrl;
        private string _username;
        private string _password;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;

        /// <summary>
        /// Gets or sets a value indicating whether the app is currently running in demo mode
        /// </summary>
        public bool? IsRunningInDemoMode
        {
            get
            {
                return _isRunningInDemoMode;
            }

            set
            {
                if (_isRunningInDemoMode == value)
                {
                    return;
                }

                _isRunningInDemoMode = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the url to the OpenHAB server
        /// </summary>
        public string OpenHABUrl
        {
            get
            {
                return _openHABUrl;
            }

            set
            {
                if (_openHABUrl == value)
                {
                    return;
                }

                _openHABUrl = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the url to the OpenHAB remote url
        /// </summary>
        public string OpenHABRemoteUrl
        {
            get
            {
                return _openHABRemoteUrl;
            }

            set
            {
                if (_openHABRemoteUrl == value)
                {
                    return;
                }

                _openHABRemoteUrl = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the username for the OpenHAB server connection
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (_username == value)
                {
                    return;
                }

                _username = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the password for the OpenHAB connection
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password == value)
                {
                    return;
                }

                _password = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL certificate
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get
            {
                return _willIgnoreSSLCertificate;
            }

            set
            {
                if (_willIgnoreSSLCertificate == value)
                {
                    return;
                }

                _willIgnoreSSLCertificate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL hostname
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get
            {
                return _willIgnoreSSLHostname;
            }

            set
            {
                if (_willIgnoreSSLHostname == value)
                {
                    return;
                }

                _willIgnoreSSLHostname = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            WillIgnoreSSLCertificate = false;
            WillIgnoreSSLHostname = false;
            IsRunningInDemoMode = false;
        }
    }
}
