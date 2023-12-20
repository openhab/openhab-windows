using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openHAB.Core.Services
{
    /// <summary>
    /// Represents the paths used by the application.
    /// </summary>
    public class AppPaths
    {
        private const string _applicationName = "openHAB";

        /// <summary>
        /// Gets the directory path for application data.
        /// </summary>
        public string ApplicationDatacfDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _applicationName);

        /// <summary>
        /// Gets the directory path for icon cache.
        /// </summary>
        public string IconCacheDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _applicationName, "icons");

        /// <summary>
        /// Gets the directory path for logs.
        /// </summary>
        public string LogsDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _applicationName, "logs");

        /// <summary>
        /// Gets the file path for settings.
        /// </summary>
        public string SettingsFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _applicationName, "settings.json");
    }
}