using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;
using OpenHAB.Core.Model;
using OpenHAB.Core.Model.Connection;
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
        private readonly IOpenHAB _openHabsdk;
        private readonly OpenHABHttpClientType _type;
        private string _password;
        private IConnectionProfile _profile;
        private ICommand _selectProfile;
        private string _url;
        private ICommand _urlCheckCommand;
        private OpenHABUrlState _urlState;
        private string _username;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionConfigViewModel"/> class.
        /// </summary>
        /// <param name="connectionConfig">The connection configuration.</param>
        /// <param name="openHabsdk">OpenHABSDK class.</param>
        public ConnectionConfigViewModel(OpenHABConnection connectionConfig, IOpenHAB openHabsdk, OpenHABHttpClientType type)
            : base(connectionConfig)
        {
            _openHabsdk = openHabsdk;
            _type = type;
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
            get
            {
                return Model?.Password;
            }

            set
            {
                Set(ref _password, value);
                Model.Password = value;
            }
        }

        /// <summary>Gets or sets the profile for the connection.</summary>
        /// <value>The profile.</value>
        public IConnectionProfile Profile
        {
            get
            {
                return Model?.Profile;
            }

            set
            {
                Set(ref _profile, value);
                Model.Profile = value;
            }
        }

        /// <summary>Gets the available connection profiles.</summary>
        /// <value>The profiles.</value>
        public ObservableCollection<IConnectionProfile> Profiles
        {
            get
            {
                return new ObservableCollection<IConnectionProfile>(Settings.ConnectionProfiles.Where(x => x.Type == _type).OrderBy(x => x.Id).ToList());
            }
        }

        /// <summary>
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SelectProfile => _selectProfile ?? (_selectProfile = new ActionCommand(CreateProfile));

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

                return "Unsecure connected to " + _url;
            }
        }

        /// <summary>
        /// Gets or sets the url to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get
            {
                return Model?.Url;
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
                Model.Url = _url;

                OnPropertyChanged(nameof(Subtitle));
            }
        }

        /// <summary>
        /// Gets the command for local url check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand UrlCheckCommand => _urlCheckCommand ?? (_urlCheckCommand = new RelayCommand<object>(CheckConnectionSettings));

        /// <summary>
        /// Gets or sets the state for OpenHab local url.
        /// </summary>
        /// <value>The state of the local URL.</value>
        public OpenHABUrlState UrlState
        {
            get => _urlState;
            set => Set(ref _urlState, value);
        }

        /// <summary>
        /// Gets or sets the username for the local OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get
            {
                return Model?.Username;
            }

            set
            {
                Set(ref _username, value);
                Model.Username = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get
            {
                return Model?.WillIgnoreSSLCertificate;
            }

            set
            {
                Set(ref _willIgnoreSSLCertificate, value);
                Model.WillIgnoreSSLCertificate = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get
            {
                return Model?.WillIgnoreSSLHostname;
            }

            set
            {
                Set(ref _willIgnoreSSLHostname, value);
                Model.WillIgnoreSSLHostname = value;
            }
        }

        private void AssignValues(OpenHABConnection connectionConfig)
        {
            Url = connectionConfig.Url;
            Username = connectionConfig.Username;
            Password = connectionConfig.Password;
            WillIgnoreSSLCertificate = connectionConfig.WillIgnoreSSLCertificate;
            WillIgnoreSSLHostname = connectionConfig.WillIgnoreSSLHostname;
        }

        private async void CheckConnectionSettings(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            Task<bool> result = _openHabsdk.CheckUrlReachability(this.Model);
            result.ContinueWith(async (task) =>
            {
                OpenHABUrlState urlState = OpenHABUrlState.Unknown;
                if (task.IsCompletedSuccessfully && task.Result)
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
            });
        }

        private void CreateProfile(IConnectionProfile value)
        {
            Model = value.CreateConnection();
            Model.Profile = value;

            AssignValues(Model);
        }

        private void CreateProfile(object obj)
        {
            IConnectionProfile profile = obj as IConnectionProfile;

            if (profile != null)
            {
                CreateProfile(profile);

                OnPropertyChanged(nameof(AllowHostUrlConfiguration));
                OnPropertyChanged(nameof(AllowIgnoreSSLCertificate));
                OnPropertyChanged(nameof(AllowIgnoreSSLHostname));
            }
        }
    }
}
