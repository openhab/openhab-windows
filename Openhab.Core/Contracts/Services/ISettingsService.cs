using System.Threading.Tasks;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.Contracts.Services
{
    /// <summary>
    /// Service to fetch / save user-defined settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets or sets the version of openHAB that's running on the server.
        /// </summary>
        OpenHABVersion ServerVersion { get; set; }

        /// <summary>
        /// Persists the current settings.
        /// </summary>
        /// <param name="settings">Current settings.</param>
        void Save(Settings settings);

        /// <summary>
        /// Load the settings.
        /// </summary>
        /// <returns>Previously saved settings.</returns>
        Settings Load();

        /// <summary>
        /// Save the name of the last opened sitemap.
        /// </summary>
        /// <param name="name">Name of the sitemap.</param>
        void SaveCurrentSitemap(string name);

        /// <summary>
        /// Loads the name of the last opened sitemap.
        /// </summary>
        /// <returns>The name of the sitemap.</returns>
        string LoadLastSitemap();
    }
}
