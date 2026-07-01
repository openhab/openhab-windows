using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace openHAB.Core.Model;

/// <summary>
/// Class that holds all the OpenHAB Windows app settings.
/// </summary>
public class SettingOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingOptions"/> class.
    /// </summary>
    public SettingOptions()
    {
        ShowDefaultSitemap = false;
        UseSVGIcons = false;
        NotificationsEnable = false;
        StartAppMinimized = false;
    }

    /// <summary>
    /// Gets or sets the application language.
    /// </summary>
    /// <value>The application language.</value>
    public string? AppLanguage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the last sitemap name in settings.
    /// </summary>
    /// <value>
    /// The last sitemap.
    /// </value>
    public string LastSitemap
    {
        get; set;
    }


    /// <summary>
    /// Gets or sets the setting to enable notifications.
    /// </summary>
    /// <value>The enable notifications.</value>
    public bool NotificationsEnable
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the default sitemap should be visible.
    /// </summary>
    /// <value>The hide default sitemap.</value>
    public bool ShowDefaultSitemap
    {
        get;
        set;
    }

    /// <summary>Gets or sets the setting to start application minimized.</summary>
    /// <value>The start application minimized.</value>
    public bool StartAppMinimized
    {
        get;
        set;
    }

    /// <summary>Gets or sets a value indicating whether [use SVG icons].</summary>
    /// <value>
    ///   <c>true</c> if [use SVG icons]; otherwise, <c>false</c>.</value>
    public bool UseSVGIcons
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the application theme.
    /// </summary>
    /// <value>The application theme.</value>
    public AppTheme AppTheme
    {
        get;
        set;
    }

    /// <summary>
    /// Saves the current settings to the specified file.
    /// </summary>
    /// <param name="filePath">The full path of the settings file to write.</param>
    /// <param name="logger">Optional logger used to record a failed save.</param>
    /// <returns><c>true</c> if the settings were saved successfully; otherwise, <c>false</c>.</returns>
    public bool Save(string filePath, ILogger logger = null)
    {
        try
        {
            string settingsContent = JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, settingsContent, Encoding.UTF8);

            return true;
        }
        catch (System.Exception ex)
        {
            logger?.LogError(ex, "Failed to save settings to '{FilePath}'.", filePath);
            return false;
        }
    }
}
