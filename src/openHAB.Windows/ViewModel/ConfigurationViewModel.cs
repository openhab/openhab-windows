using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using openHAB.Core;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Options;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using ConnectionState = openHAB.Core.Client.Connection.Models.ConnectionState;

namespace openHAB.Windows.ViewModel;

/// <summary>
/// Class that holds all the OpenHAB Windows application settings.
/// </summary>
public class ConfigurationViewModel : ViewModelBase<object>
{
    private readonly IAppManager _appManager;
    private readonly ConnectionOptions _connection;
    private readonly ILogger<ConfigurationViewModel> _logger;
    private readonly SettingOptions _settings;
    private List<LanguageViewModel> _appLanguages;
    private bool? _canAppAutostartEnabled;
    private bool? _isAppAutostartEnabled;
    private bool? _isRunningInDemoMode;
    private ConnectionDialogViewModel _localConnection;
    private bool _notificationsEnable;
    private ConnectionDialogViewModel _remoteConnection;
    private LanguageViewModel _selectedAppLanguage;

    private ApplicationThemeViewModel _selectedAppTheme;
    private bool _showDefaultSitemap;
    private bool _startAppMinimized;
    private bool _useSVGIcons;
    private List<ApplicationThemeViewModel> _appThemes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationViewModel"/> class.
    /// </summary>
    /// <param name="appManager">The application manager.</param>
    /// <param name="connectionService">The connection service.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="settingsOptions">The settings options.</param>
    /// <param name="connectionOptions">The connection options.</param>
    public ConfigurationViewModel(
        IAppManager appManager,
        IConnectionService connectionService,
        ILogger<ConfigurationViewModel> logger,
        IOptions<SettingOptions> settingsOptions,
        IOptions<ConnectionOptions> connectionOptions)
        : base(new object())
    {
        _logger = logger;
        _settings = settingsOptions.Value;
        _connection = connectionOptions.Value;
        _appManager = appManager;

        _localConnection = new ConnectionDialogViewModel(_connection.LocalConnection, connectionService, HttpClientType.Local);
        _localConnection.PropertyChanged += ConnectionPropertyChanged;

        _remoteConnection = new ConnectionDialogViewModel(_connection.RemoteConnection, connectionService, HttpClientType.Remote);
        _remoteConnection.PropertyChanged += ConnectionPropertyChanged;

        _isRunningInDemoMode = _connection.IsRunningInDemoMode;
        _showDefaultSitemap = _settings.ShowDefaultSitemap;
        _useSVGIcons = _settings.UseSVGIcons;
        _startAppMinimized = _settings.StartAppMinimized;
        _notificationsEnable = _settings.NotificationsEnable;

        _appThemes = Enum.GetValues<AppTheme>().Select(x => new ApplicationThemeViewModel(x)).ToList();
        _selectedAppTheme = _appThemes.FirstOrDefault(x => x.Model == _settings.AppTheme);

        InitializeAutostartSettings();
        InitializeAppLanguage();

        PropertyChanged += ConfigurationViewModel_PropertyChanged;
        OnPropertyChanged(nameof(SelectedAppTheme));
    }

    #region Properties

    /// <summary>
    /// Gets or sets the supported application languages.
    /// </summary>
    /// <value>The application languages.</value>
    public List<LanguageViewModel> AppLanguages
    {
        get => _appLanguages;
        set => Set(ref _appLanguages, value);
    }

    /// <summary>
    /// Gets or sets the supported application themes.
    /// </summary>
    /// <value>The application themes.</value>
    public List<ApplicationThemeViewModel> AppThemes
    {
        get => _appThemes;
        set => Set(ref _appThemes, value);
    }

    /// <summary>
    /// Gets or sets the can application autostart enabled.
    /// </summary>
    /// <value>The can application autostart enabled.</value>
    public bool? CanAppAutostartEnabled
    {
        get => _canAppAutostartEnabled;
        set => App.DispatcherQueue.EnqueueAsync(() =>
        {
            _canAppAutostartEnabled = value;
            OnPropertyChanged(nameof(CanAppAutostartEnabled));
        });
    }

    /// <summary>
    /// Gets or sets the is application autostart is enabled.
    /// </summary>
    /// <value>The is application autostart enabled.</value>
    public bool? IsAppAutostartEnabled
    {
        get => _isAppAutostartEnabled;
        set => App.DispatcherQueue.EnqueueAsync(() =>
        {
            _isAppAutostartEnabled = value;
            OnPropertyChanged(nameof(IsAppAutostartEnabled));
        });
    }

    /// <summary>
    /// Gets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    public new bool IsDirty => base.IsDirty || LocalConnection.IsDirty || RemoteConnection.IsDirty;

    /// <summary>
    /// Gets or sets a value indicating whether the app is currently running in demo mode.
    /// </summary>
    public bool? IsRunningInDemoMode
    {
        get => _isRunningInDemoMode;
        set
        {
            if (Set(ref _isRunningInDemoMode, value, true))
            {
                _connection.IsRunningInDemoMode = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets local OpenHAB connection configuration.
    /// </summary>
    public ConnectionDialogViewModel LocalConnection
    {
        get => _localConnection;
        set
        {
            if (Set(ref _localConnection, value))
            {
                _connection.LocalConnection = value.Model;
            }
        }
    }

    /// <summary>
    /// Gets or sets the setting if notifications are enabled.
    /// </summary>
    /// <value>The application triggers notification on openHAB events.</value>
    public bool NotificationsEnable
    {
        get => _notificationsEnable;
        set
        {
            if (Set(ref _notificationsEnable, value, true))
            {
                _settings.NotificationsEnable = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets remote OpenHAB connection configuration.
    /// </summary>
    public ConnectionDialogViewModel RemoteConnection
    {
        get => _remoteConnection;
        set
        {
            if (Set(ref _remoteConnection, value))
            {
                _connection.RemoteConnection = value.Model;
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected application language.
    /// </summary>
    /// <value>The selected application language.</value>
    public LanguageViewModel SelectedAppLanguage
    {
        get => _selectedAppLanguage;
        set
        {
            if (Set(ref _selectedAppLanguage, value, true))
            {
                _settings.AppLanguage = value.Code;
            }
        }
    }

    public ApplicationThemeViewModel SelectedAppTheme
    {
        get => _selectedAppTheme;
        set
        {
            if (value != null)
            {
                _settings.AppTheme = value.Model;
                Set<ApplicationThemeViewModel>(ref _selectedAppTheme, value, true);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the default sitemap should be visible.
    /// </summary>
    /// <value>The hide default sitemap.</value>
    public bool ShowDefaultSitemap
    {
        get => _showDefaultSitemap;
        set
        {
            if (Set(ref _showDefaultSitemap, value, true))
            {
                _settings.ShowDefaultSitemap = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the start application minimized.
    /// </summary>
    /// <value>The start application minimized.</value>
    public bool StartAppMinimized
    {
        get => _startAppMinimized;
        set
        {
            if (Set(ref _startAppMinimized, value, true))
            {
                _settings.StartAppMinimized = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [use SVG icons].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [use SVG icons]; otherwise, <c>false</c>.</value>
    public bool UseSVGIcons
    {
        get => _useSVGIcons;
        set
        {
            if (Set(ref _useSVGIcons, value, true))
            {
                _settings.UseSVGIcons = value;
            }
        }
    }

    #endregion

    /// <summary>
    /// Determines whether [is connection configuration valid].
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [is connection configuration valid]; otherwise, <c>false</c>.</returns>
    public bool IsConnectionConfigValid()
    {
        bool validConfig = IsRunningInDemoMode.GetValueOrDefault() ||
                 (!string.IsNullOrEmpty(LocalConnection?.Url) && LocalConnection?.Status.State == ConnectionState.OK) ||
                 (!string.IsNullOrEmpty(RemoteConnection?.Url) && RemoteConnection?.Status.State == ConnectionState.OK);

        _logger.LogInformation("Valid application configuration: {ValidConfig}", validConfig);

        return validConfig;
    }

    /// <summary>
    /// Persists the settings to disk.
    /// </summary>
    /// <returns>True if settings saved successful, otherwise false.</returns>
    public bool Save()
    {
        _connection.LocalConnection = _localConnection.Model;
        _connection.RemoteConnection = _remoteConnection.Model;

        bool result = _settings.Save(AppPaths.SettingsFilePath, _logger);
        result &= _connection.Save(AppPaths.ConnectionFilePath, _logger);
        _appManager.SetProgramLanguage(null);
        _appManager.SetAppTheme(App.MainWindow.Content);

        return result;
    }

    private void ConfigurationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IsDirty) && e.PropertyName != nameof(CanAppAutostartEnabled) && e.PropertyName != nameof(IsAppAutostartEnabled))
        {
            OnPropertyChanged(nameof(IsDirty));
        }
    }

    private void ConnectionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsDirty));
    }

    private void InitializeAppLanguage()
    {
        _appLanguages = InitializeAppLanguages();

        _selectedAppLanguage = string.IsNullOrEmpty(_settings.AppLanguage)
            ? _appLanguages.FirstOrDefault(x => x.Code == null)
            : _appLanguages.FirstOrDefault(x => string.Equals(x.Code, _settings.AppLanguage, StringComparison.InvariantCultureIgnoreCase));
    }

    private List<LanguageViewModel> InitializeAppLanguages()
    {
        return new List<LanguageViewModel>
        {
            new LanguageViewModel { Name = "System", Code = null },
            new LanguageViewModel { Name = "English", Code = "en-US" },
            new LanguageViewModel { Name = "Deutsch", Code = "de-DE" }
        };
    }

    private async void InitializeAutostartSettings()
    {
        CanAppAutostartEnabled = await _appManager.CanEnableAutostart();
        IsAppAutostartEnabled = await _appManager.IsStartupEnabled();
    }
}
