using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// Class that holds all the OpenHAB Windows app settings.
    /// </summary>
    public class ConfigurationViewModel : ViewModelBase<object>
    {
        private bool? _hideDefaultSitemap;
        private bool? _isRunningInDemoMode;
        private ConnectionConfigViewModel _remoteConnection;
        private ConnectionConfigViewModel _localConnection;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<ConfigurationViewModel> _logger;
        private readonly Settings _settings;
        private bool? _willIgnoreSSLCertificate;
        private bool? _willIgnoreSSLHostname;
        private LanguageViewModel _selectedAppLanguage;
        private List<LanguageViewModel> _appLanguages;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationViewModel"/> class.
        /// </summary>
        public ConfigurationViewModel(ISettingsService settingsService, IOpenHAB openHabsdk, ILogger<ConfigurationViewModel> logger) 
            : base(new object())
        {
            _settingsService = settingsService;
            _logger = logger;
            _settings = settingsService.Load();

            _localConnection = new ConnectionConfigViewModel(_settings.LocalConnection, openHabsdk);
            _remoteConnection = new ConnectionConfigViewModel(_settings.RemoteConnection, openHabsdk);

            _willIgnoreSSLCertificate = _settings.WillIgnoreSSLCertificate;
            _willIgnoreSSLHostname = _settings.WillIgnoreSSLHostname;
            _isRunningInDemoMode = _settings.IsRunningInDemoMode;
            _hideDefaultSitemap = _settings.HideDefaultSitemap;

            _appLanguages = InitalizeAppLanguages();

            _selectedAppLanguage =
                _appLanguages.FirstOrDefault(x => string.Compare(x.Code, _settings.AppLanguage, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        private List<LanguageViewModel> InitalizeAppLanguages()
        {
            List<LanguageViewModel> appLanguages = new List<LanguageViewModel>();
            LanguageViewModel system = new LanguageViewModel()
            {
                Name = "System",
                Code = null
            };

            LanguageViewModel english = new LanguageViewModel()
            {
                Name = "English",
                Code = "en-us"
            };

            LanguageViewModel german = new LanguageViewModel()
            {
                Name = "Deutsch",
                Code = "de-de"
            };

            appLanguages.Add(system);
            appLanguages.Add(english);
            appLanguages.Add(german);

            return appLanguages;
        }

        /// <summary>,
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
                Set(ref _hideDefaultSitemap, value);
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
                Set(ref _isRunningInDemoMode, value);
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
                Set(ref _remoteConnection, value);
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
                Set(ref _localConnection, value);
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
                Set(ref _willIgnoreSSLCertificate, value);
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
                Set(ref _willIgnoreSSLHostname, value);
                _settings.WillIgnoreSSLHostname = value;
            }
        }

        /// <summary>
        /// Gets or sets the supported application languages.
        /// </summary>
        /// <value>The application languages.</value>
        public List<LanguageViewModel> AppLanguages
        {
            get
            {
                return _appLanguages;
            }

            set
            {
                Set(ref _appLanguages, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected application language.
        /// </summary>
        /// <value>The selected application language.</value>
        public LanguageViewModel SelectedAppLanguage
        {
            get
            {
                return _selectedAppLanguage;
            }

            set
            {
                Set(ref _selectedAppLanguage, value);
                _settings.AppLanguage = value.Code;
            }
        }

        /// <summary>Determines whether [is connection configuration valid].</summary>
        /// <returns>
        ///   <c>true</c> if [is connection configuration valid]; otherwise, <c>false</c>.</returns>
        public bool IsConnectionConfigValid()
        {
            bool validConfig = IsRunningInDemoMode.Value ||
                     !string.IsNullOrEmpty(LocalConnection?.Url) ||
                     !string.IsNullOrEmpty(RemoteConnection?.Url);

            _logger.LogInformation($"Vaild app configuration: {validConfig}");

            return validConfig;
        }

        /// <summary>
        /// Persists the settings to disk.
        /// </summary>
        public void Save()
        {
            _settingsService.Save(_settings);
            _settingsService.SetProgramLanguage(null);
        }
    }
}
