using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using openHAB.Core.Common;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using Windows.Globalization;
using Windows.Storage;
using Windows.System.UserProfile;

namespace openHAB.Core.Services
{
    /// <summary>
    /// Service that handles all settings operations.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private ILogger<SettingsService> _logger;
        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        private ApplicationDataContainer _settingsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService(ILogger<SettingsService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public OpenHABVersion ServerVersion
        {
            get; set;
        }

        /// <inheritdoc />
        public Settings Load()
        {
            EnsureSettingsContainer();

            if (!_settingsContainer.Values.ContainsKey(Constants.Local.SettingsKey))
            {
                return new Settings();
            }

            string json = _settingsContainer.Values[Constants.Local.SettingsKey].ToString();

            if (json == null)
            {
                return new Settings();
            }

            // Fix to ensure the settings can still loaded after project restructing
            // TODO: Remove this part in future
            if (json.Contains("OpenHAB.Core.Model.Connection"))
            {
                json = json.Replace("OpenHAB.Core.Model.Connection", "openHAB.Core.Connection");
            }

            return JsonConvert.DeserializeObject<Settings>(json, _serializerSettings);
        }

        /// <inheritdoc />
        public string LoadLastSitemap()
        {
            _logger.LogInformation("Load last selected sitemape from settings");

            if (!_settingsContainer.Values.TryGetValue(Constants.Local.SitemapKey, out object sitemapKey))
            {
                return null;
            }

            return sitemapKey.ToString();
        }

        /// <inheritdoc />
        public bool Save(Settings settings)
        {
            try
            {
                _logger.LogInformation("Save settings to disk");

                EnsureSettingsContainer();
                _settingsContainer.Values[Constants.Local.SettingsKey] = JsonConvert.SerializeObject(settings, _serializerSettings);

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to save settings.");
                return false;
            }
        }

        /// <inheritdoc />
        public void SaveCurrentSitemap(string name)
        {
            _logger.LogInformation("Update selected sitemape in settings");

            _settingsContainer.Values[Constants.Local.SitemapKey] = name;
        }

        /// <inheritdoc />
        public void SetProgramLanguage(string langcode)
        {
            if (string.IsNullOrEmpty(langcode))
            {
                Settings settings = Load();
                langcode = settings.AppLanguage;
            }

            if (!string.IsNullOrEmpty(langcode))
            {
                ApplicationLanguages.PrimaryLanguageOverride = langcode;
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];
            }
        }

        private void EnsureSettingsContainer()
        {
            if (_settingsContainer == null)
            {
                _settingsContainer = ApplicationData.Current.RoamingSettings;
            }
        }
    }
}
