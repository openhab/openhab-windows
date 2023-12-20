using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using Windows.System.UserProfile;

namespace openHAB.Core.Services
{
    /// <summary>
    /// Service that handles all settings operations.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private AppPaths _applicationContext;
        private ILogger<SettingsService> _logger;
        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        public SettingsService(AppPaths applicationContext, ILogger<SettingsService> logger)
        {
            _applicationContext = applicationContext;
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
            if (!File.Exists(_applicationContext.SettingsFilePath))
            {
                return new Settings();
            }

            _logger.LogInformation("Load settings from disk");

            string fileContent = File.ReadAllText(_applicationContext.SettingsFilePath, Encoding.UTF8);

            if (string.IsNullOrEmpty(fileContent))
            {
                return new Settings();
            }

            // Fix to ensure the settings can still loaded after project restructing
            // TODO: Remove this part in future
            if (fileContent.Contains("OpenHAB.Core.Model.Connection"))
            {
                fileContent = fileContent.Replace("OpenHAB.Core.Model.Connection", "openHAB.Core.Connection");
            }

            return JsonConvert.DeserializeObject<Settings>(fileContent, _serializerSettings);
        }

        /// <inheritdoc />
        public bool Save(Settings settings)
        {
            try
            {
                _logger.LogInformation("Save settings to disk");

                string settingsContent = JsonConvert.SerializeObject(settings, _serializerSettings);
                File.WriteAllText(_applicationContext.SettingsFilePath, settingsContent, Encoding.UTF8);

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to save settings.");
                return false;
            }
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
                CultureInfo.CurrentCulture = new CultureInfo(langcode);
            }
            else
            {
                string userLanguage = GlobalizationPreferences.Languages[0];
                CultureInfo.CurrentCulture = new CultureInfo(userLanguage);
            }
        }
    }
}
