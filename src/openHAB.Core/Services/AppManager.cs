using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using Windows.ApplicationModel;
using Windows.System.UserProfile;

namespace openHAB.Core.Services;

/// <inheritdoc/>
public class AppManager : IAppManager
{
    private readonly ILogger<AppManager> _logger;
    private readonly IOptions<SettingOptions> _options;
    private readonly string _startupId = "openHABStartupId";

    /// <summary>
    /// Initializes a new instance of the <see cref="AppManager" /> class.
    /// </summary>
    /// <param name="options">The settings options.</param>
    /// <param name="logger">The logger instance.</param>
    public AppManager(IOptions<SettingOptions> options, ILogger<AppManager> logger)
    {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc />
    public OpenHABVersion ServerVersion
    {
        get; set;
    }

    /// <inheritdoc/>
    public async Task<bool> CanEnableAutostart()
    {
        StartupTask startupTask = await StartupTask.GetAsync(_startupId);
        return !(startupTask.State == StartupTaskState.DisabledByPolicy || startupTask.State == StartupTaskState.DisabledByUser);
    }

    /// <inheritdoc/>
    public async Task<bool> IsStartupEnabled()
    {
        StartupTask startupTask = await StartupTask.GetAsync(_startupId);
        return startupTask.State == StartupTaskState.Enabled || startupTask.State == StartupTaskState.EnabledByPolicy;
    }

    /// <inheritdoc />
    public void SetProgramLanguage(string langcode)
    {
        if (string.IsNullOrEmpty(langcode))
        {
            SettingOptions settings = _options.Value;
            langcode = settings.AppLanguage;
        }

        if (!string.IsNullOrEmpty(langcode))
        {
            CultureInfo.CurrentCulture = new CultureInfo(langcode);
            CultureInfo.CurrentUICulture = new CultureInfo(langcode);
        }
        else
        {
            langcode = GlobalizationPreferences.Languages[0];
            CultureInfo.CurrentCulture = new CultureInfo(langcode);
            CultureInfo.CurrentUICulture = new CultureInfo(langcode);
        }

        //Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = langcode;
    }

    [DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
    public static extern bool ShouldSystemUseDarkMode();

    /// <inheritdoc />
    public void SetAppTheme(UIElement content)
    {
        if (!(content is FrameworkElement frameworkElement))
        {
            _logger.LogWarning("Unable to set app theme");
            return;
        }

        SettingOptions settings = _options.Value;
        switch (settings.AppTheme)
        {
            case AppTheme.Light:
                frameworkElement.RequestedTheme = ElementTheme.Light;
                break;
            case AppTheme.Dark:
                frameworkElement.RequestedTheme = ElementTheme.Dark;
                break;
            case AppTheme.System:
                frameworkElement.RequestedTheme = ShouldSystemUseDarkMode() ? ElementTheme.Dark : ElementTheme.Light;
                break;
            default:
                break;
        }
    }

    /// <inheritdoc/>
    public async Task ToggleAutostart()
    {
        StartupTask startupTask = await StartupTask.GetAsync(_startupId);
        switch (startupTask.State)
        {
            case StartupTaskState.DisabledByPolicy:
            case StartupTaskState.DisabledByUser:
            case StartupTaskState.Disabled:
                StartupTaskState newState = await startupTask.RequestEnableAsync();
                _logger.LogInformation($"App autostart: {newState.ToString()}");
                break;

            case StartupTaskState.EnabledByPolicy:
            case StartupTaskState.Enabled:
                startupTask.Disable();
                _logger.LogInformation($"App autostart: disabled");
                break;
        }
    }
}
