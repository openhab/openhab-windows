using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Common;
using openHAB.Core.Contracts;
using openHAB.Core.Model;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for OpenHAB connection dialog.
    /// </summary>
    public class ConnectionDialogViewModel : ViewModelBase<Connection>
    {
        private readonly HttpClientType _type;
        private string _password;
        private ConnectionProfileViewModel _profile;
        private ObservableCollection<ConnectionProfileViewModel> _profiles;
        private ICommand _selectProfile;
        private string _url;
        private ICommand _urlCheckCommand;
        private ConnectionStatusViewModel _connectionStatus;
        private string _username;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDialogViewModel"/> class.
        /// </summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        /// <param name="connectionService">ConnectionService to retrive connection information.</param>
        /// <param name="type">Defines if openHAB instance is local or remote.</param>
        public ConnectionDialogViewModel(Connection connectionConfig, IConnectionService connectionService, HttpClientType type)
            : base(connectionConfig)
        {
            _type = type;

            List<ConnectionProfileViewModel> list
                = new List<ConnectionProfileViewModel>(Settings.ConnectionProfiles.Where(x => x.Type == _type).OrderBy(x => x.Id).Select(x => new ConnectionProfileViewModel(x)));

            _profiles = new ObservableCollection<ConnectionProfileViewModel>(list);

            if (Model != null)
            {
                _profile = list.FirstOrDefault(x => x.Id == Model.Profile.Id);
            }

            _connectionStatus = new ConnectionStatusViewModel(connectionService);

            if (!string.IsNullOrEmpty(Model?.Url))
            {
                CheckConnectionSettings(null);
            }

            _username = Model?.Username;
            _password = Model?.Password;
            _url = Model?.Url;
            _willIgnoreSSLHostname = Model?.WillIgnoreSSLHostname;
            _willIgnoreSSLCertificate = Model?.WillIgnoreSSLCertificate;
        }

        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        public bool AllowHostUrlConfiguration
        {
            get => Profile?.AllowHostUrlConfiguration ?? false;
        }

        /// <summary>Gets a value indicating whether [allow ignore SSL certificate] issues option is available.</summary>
        /// <value>
        ///   <c>true</c> if [allow ignore SSL certificate] is available; otherwise, <c>false</c>.</value>
        public bool AllowIgnoreSSLCertificate
        {
            get => Profile?.AllowIgnoreSSLCertificate ?? false;
        }

        /// <summary>Gets a value indicating whether [host URL] value can be modified.</summary>
        /// <value>
        ///   <c>true</c> if [host URL configuration] can be modified; otherwise, <c>false</c>.</value>
        public bool AllowIgnoreSSLHostname
        {
            get => Profile?.AllowIgnoreSSLHostname ?? false;
        }

        /// <summary>
        /// Gets or sets the password for the local OpenHAB connection.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                Set(ref _password, value, true);
                Model.Password = value;
            }
        }

        /// <summary>Gets or sets the profile for the connection.</summary>
        /// <value>The profile.</value>
        public ConnectionProfileViewModel Profile
        {
            get => _profile;
            set
            {
                Set(ref _profile, value);

                if (Model != null && value != null)
                {
                    Model.Profile = value.Model;
                }
            }
        }

        /// <summary>Gets or sets the connection status.</summary>
        /// <value>The connection status.</value>
        public ConnectionStatusViewModel Status
        {
            get => _connectionStatus;
            set
            {
                Set(ref _connectionStatus, value);
            }
        }

        /// <summary>Gets or sets the available connection profiles.</summary>
        /// <value>The profiles.</value>
        public ObservableCollection<ConnectionProfileViewModel> Profiles
        {
            get => _profiles;
            set
            {
                Set(ref _profiles, value);
            }
        }

        /// <summary>
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SelectProfile => _selectProfile ?? (_selectProfile = new ActionCommand(ExecuteSelectProfile));

        /// <summary>
        /// Gets the subtitle.
        /// </summary>
        /// <value>The subtitle.</value>
        public string Subtitle
        {
            get
            {
                if (string.IsNullOrEmpty(Url))
                {
                    return AppResources.Values.GetString("ConnectionNotConfigured");
                }

                if (Uri.TryCreate(Url, UriKind.Absolute, out Uri uri) &&
                    string.Compare(uri.Scheme.ToUpperInvariant(), "HTTPS", StringComparison.InvariantCulture) == 0)
                {
                    return $"{AppResources.Values.GetString("EncryptedConnection")} {Url}";
                }

                return $"{AppResources.Values.GetString("UnencryptedConnection")} {Url}";
            }
        }

        /// <summary>
        /// Gets or sets the URL to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get => _url;
            set
            {
                string tempUrl;
                if (!string.IsNullOrEmpty(value) && !value.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
                {
                    tempUrl = value + "/";
                }
                else
                {
                    tempUrl = value;
                }

                Set(ref _url, tempUrl, true);
                Model.Url = _url;

                OnPropertyChanged(nameof(Subtitle));
            }
        }

        /// <summary>
        /// Gets the command for local URL check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand UrlCheckCommand => _urlCheckCommand ?? (_urlCheckCommand = new RelayCommand<object>(CheckConnectionSettings));

        /// <summary>
        /// Gets or sets the username for the local OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                Set(ref _username, value, true);
                Model.Username = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get => _willIgnoreSSLCertificate;
            set
            {
                Set(ref _willIgnoreSSLCertificate, value, true);
                Model.WillIgnoreSSLCertificate = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the application will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get => _willIgnoreSSLHostname;
            set
            {
                Set(ref _willIgnoreSSLHostname, value, true);
                Model.WillIgnoreSSLHostname = value;
            }
        }

        private void CheckConnectionSettings(object parameter)
        {
            _connectionStatus.CheckConnectionSettings(Model.Url, Model);
        }

        private void CreateProfile(IConnectionProfile value)
        {
            Model = value.CreateConnection();
            Model.Profile = value;

            Url = Model.Url;
            Username = Model.Username;
            Password = Model.Password;
            WillIgnoreSSLCertificate = Model.WillIgnoreSSLCertificate;
            WillIgnoreSSLHostname = Model.WillIgnoreSSLHostname;
        }

        private void ExecuteSelectProfile(object obj)
        {
            ConnectionProfileViewModel profile = obj as ConnectionProfileViewModel;

            if (profile != null)
            {
                CreateProfile(profile.Model);

                OnPropertyChanged(nameof(AllowHostUrlConfiguration));
                OnPropertyChanged(nameof(AllowIgnoreSSLCertificate));
                OnPropertyChanged(nameof(AllowIgnoreSSLHostname));
            }
        }
    }
}
