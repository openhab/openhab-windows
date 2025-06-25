using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using openHAB.Core.Common;
using openHAB.Core.Messages;
using openHAB.Core.Services.Contracts;
using Windows.ApplicationModel;

namespace openHAB.Windows.ViewModel;

/// <summary>
/// Collects and formats all the data for user defined settings.
/// </summary>
public class SettingsViewModel : ViewModelBase<object>
{
    private readonly IIconCaching _iconCaching;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IAppManager _appManager;
    private ActionCommand _clearIconCacheCommand;
    private ConfigurationViewModel _configuration;
    private ActionCommand _saveCommand;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="configurationViewModel">The configuration view model containing user settings.</param>
    /// <param name="iconCaching">The icon caching service.</param>
    /// <param name="appManager">The application manager service.</param>
    /// <param name="logger">The logger instance.</param>
    public SettingsViewModel(
        ConfigurationViewModel configurationViewModel,
        IIconCaching iconCaching,
        IAppManager appManager,
        ILogger<SettingsViewModel> logger)
        : base(new object())
    {
        _configuration = configurationViewModel;
        _configuration.PropertyChanged += Configuration_PropertyChanged;
        _appManager = appManager;
        _iconCaching = iconCaching;

        _logger = logger;

        StrongReferenceMessenger.Default.Register<ConnectionStatusChanged>(this, (recipient, msg) => SaveCommand.InvokeCanExecuteChanged(null));
    }

    /// <summary>
    /// Gets the command to clear the icon cache.
    /// </summary>
    public ActionCommand ClearIconCacheCommand => _clearIconCacheCommand ?? (_clearIconCacheCommand = new ActionCommand(ClearIcons, CanClearIcons));

    /// <summary>
    /// Gets the command to save and persist the settings.
    /// </summary>
    public ActionCommand SaveCommand => _saveCommand ?? (_saveCommand = new ActionCommand(PersistSettings, CanPersistSettings));

    /// <summary>
    /// Gets or sets the current user-defined settings.
    /// </summary>
    public ConfigurationViewModel Settings
    {
        get => _configuration;
        set => Set(ref _configuration, value);
    }

    /// <summary>
    /// Gets the application version number.
    /// </summary>
    public string Version
    {
        get
        {
            Version version = new Version(
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build,
                Package.Current.Id.Version.Revision);

            return version.ToString();
        }
    }

    /// <summary>
    /// Saves the user defined settings to the UWP settings storage.
    /// </summary>
    /// <param name="obj">Optional command parameter (not used).</param>
    public void PersistSettings(object obj)
    {
        _logger.LogInformation("Execute save settings command");

        bool validConnectionConfig = CheckForValidConnectionConfig();
        if (validConnectionConfig)
        {
            bool savedSuccessful = _configuration.Save();

            _appManager.SetAppTheme(App.MainWindow.Content);
            StrongReferenceMessenger.Default.Send<TriggerAction>(new TriggerAction(Core.Messages.Action.Reload));
        }
    }

    private bool CheckForValidConnectionConfig()
    {
        return _configuration.IsConnectionConfigValid() ||
           (_configuration.IsRunningInDemoMode.HasValue && _configuration.IsRunningInDemoMode.Value);
    }

    private bool CanClearIcons(object arg)
    {
        return true;
    }

    private bool CanPersistSettings(object arg)
    {
        bool validConnectionConfig = CheckForValidConnectionConfig();

        return validConnectionConfig && _configuration.IsDirty;
    }

    private void ClearIcons(object obj)
    {
        _iconCaching.ClearIconCache();
    }

    private void Configuration_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        SaveCommand.InvokeCanExecuteChanged(null);
    }
}
