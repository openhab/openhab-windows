using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.SDK;
using OpenHAB.Core.ViewModel;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Class that holds all the OpenHAB Windows app settings.
    /// </summary>
    public class ConfigurationViewModel : ObservableBase
    {
        private bool? _hideDefaultSitemap;
        private bool? _isRunningInDemoMode;
        private ConnectionConfigViewModel _remoteConnection;
        private ConnectionConfigViewModel _localConnection;
        private readonly ISettingsService _settingsService;
        private readonly Settings _settings;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationViewModel"/> class.
        /// </summary>
        public ConfigurationViewModel(ISettingsService settingsService, IOpenHAB openHabsdk)
        {
            _settingsService = settingsService;
            _settings = settingsService.Load();

            _localConnection = new ConnectionConfigViewModel(_settings.LocalConnection, openHabsdk);
            _remoteConnection = new ConnectionConfigViewModel(_settings.RemoteConnection, openHabsdk);

            _willIgnoreSSLCertificate = _settings.WillIgnoreSSLCertificate;
            _willIgnoreSSLHostname = _settings.WillIgnoreSSLHostname;
            _isRunningInDemoMode = _settings.IsRunningInDemoMode;
            _hideDefaultSitemap = _settings.HideDefaultSitemap;
        }

        /// <summary>
        /// Gets or sets the if the default sitemap should be hidden.
        /// </summary>
        /// <value>The hide default sitemap.</value>
        public bool? HideDefaultSitemap
        {
            get
            {
                return _hideDefaultSitemap;
            }

            set
            {
                SetProperty(ref _hideDefaultSitemap, value);
                _settings.HideDefaultSitemap = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the app is currently running in demo mode.
        /// </summary>
        public bool? IsRunningInDemoMode
        {
            get
            {
                return _isRunningInDemoMode;
            }

            set
            {
                SetProperty(ref _isRunningInDemoMode, value);
                _settings.IsRunningInDemoMode = value;
            }
        }

        /// <summary>
        /// Gets or sets remote OpenHAB connection configuration.
        /// </summary>
        public ConnectionConfigViewModel RemoteConnection
        {
            get
            {
                return _remoteConnection;
            }

            set
            {
                SetProperty(ref _remoteConnection, value);
            }
        }

        /// <summary>
        /// Gets or sets local OpenHAB connection configuration.
        /// </summary>
        public ConnectionConfigViewModel LocalConnection
        {
            get
            {
                return _localConnection;
            }

            set
            {
                SetProperty(ref _localConnection, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get
            {
                return _willIgnoreSSLCertificate;
            }

            set
            {
                SetProperty(ref _willIgnoreSSLCertificate, value);
                _settings.WillIgnoreSSLCertificate = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get
            {
                return _willIgnoreSSLHostname;
            }

            set
            {
                SetProperty(ref _willIgnoreSSLHostname, value);
                _settings.WillIgnoreSSLHostname = value;
            }
        }

        /// <summary>Determines whether [is connection configuration valid].</summary>
        /// <returns>
        ///   <c>true</c> if [is connection configuration valid]; otherwise, <c>false</c>.</returns>
        public bool IsConnectionConfigValid()
        {
            return IsRunningInDemoMode.Value ||
                   !string.IsNullOrEmpty(LocalConnection?.Url) ||
                   !string.IsNullOrEmpty(RemoteConnection?.Url);
        }

        /// <summary>
        /// Persists the settings to disk.
        /// </summary>
        public void Save()
        {
            _settingsService.Save(_settings);
        }
    }
}
