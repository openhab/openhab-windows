using openHAB.Core.Client.Models;
using openHAB.Core.Model;

namespace openHAB.Core.Services.Contracts
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
        /// <returns>true when settings stored successful to disk.</returns>
        bool Save(Settings settings);

        /// <summary>
        /// Load the settings.
        /// </summary>
        /// <returns>Previously saved settings.</returns>
        Settings Load();

        /// <summary>
        ///   Sets the program language.
        /// </summary>
        /// <param name="langcode">Language code e.g. en-us.</param>
        void SetProgramLanguage(string langcode);
    }
}
