using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using openHAB.Core;
using openHAB.Core.Model;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.Services.Contracts;

namespace openHAB.Windows;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private IAppManager _appManager;
    private INotificationManager _notificationManager;
    private IOptions<SettingOptions> _options;
    private ILogger<App> _logger;
    private static Window _mainWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="App" /> class.
    /// </summary>
    public App()
    {
        try
        {
            this.InitializeComponent();

            // The Generic Host is built in Program.Main, before Application.Start.
            // Building it inside this constructor runs it within the native WinUI
            // Application factory callback; any failure is then rethrown across that
            // native boundary and surfaces as an opaque COMException 0x8000FFFF
            // (E_UNEXPECTED) instead of the real exception.

            // Initialize services after the host has been built
            InitializeServices();

            // Set up exception handling and theme
            var appTheme = _options?.Value?.AppTheme ?? AppTheme.System;
            RequestedTheme = appTheme.ConvertToApplicationTheme();
            UnhandledException += App_UnhandledException;
            DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing App: {ex}");
            throw;
        }
    }

    private void InitializeServices()
    {
        _appManager = Program.Host.Services.GetRequiredService<IAppManager>();
        _notificationManager = Program.Host.Services.GetRequiredService<INotificationManager>();
        _options = Program.Host.Services.GetRequiredService<IOptions<SettingOptions>>();
        _logger = Program.Host.Services.GetRequiredService<ILogger<App>>();
    }

    /// <summary>
    /// Gets the dispatcher queue for the current thread.
    /// </summary>
    public static DispatcherQueue DispatcherQueue
    {
        get; private set;
    }

    /// <summary>
    /// Gets the main window of the application.
    /// </summary>
    public static Window MainWindow
    {
        get => _mainWindow;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user. Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="e">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs e)
    {
        _logger.LogInformation("=== Start Application ===");
        _appManager.SetProgramLanguage(null);

        AppInstance mainInstance = AppInstance.FindOrRegisterForKey("main");
        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

        DispatcherQueue = DispatcherQueue.GetForCurrentThread();

        // Register for toast activation. Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        AppNotificationManager.Default.NotificationInvoked += Default_NotificationInvoked;


        // If the instance that's executing the OnLaunched handler right now
        // isn't the "main" instance.
        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return;
        }

        if (activatedEventArgs.Kind == ExtendedActivationKind.ToastNotification)
        {

        }

        // Initialize MainWindow here
        _mainWindow = Program.Host.Services.GetRequiredService<MainWindow>();

        _appManager.SetAppTheme(MainWindow.Content);

        MainWindow.Activate();
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        _logger.LogCritical(e.Exception, "Unhandled Exception");
    }

    private void Default_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        // Use the dispatcher from the window if present, otherwise the app dispatcher
        var dispatcherQueue = MainWindow?.DispatcherQueue ?? App.DispatcherQueue;

        dispatcherQueue.TryEnqueue(delegate
        {
            var arguments = args.Arguments;

            switch (arguments["action"])
            {
                //// Send a background message
                //case "show":
                //    string message = e.UserInput["textBox"].ToString();
                //    // TODO: Send it

                //    // If the UI app isn't open
                //    if (MainWindow == null)
                //    {
                //        // Close since we're done
                //        Process.GetCurrentProcess().Kill();
                //    }

                //    break;

                // View a message
                case "show":

                    string itemName = arguments["item"];
                    // Launch/bring window to foreground
                    //LaunchAndBringToForegroundIfNeeded();

                    // TODO: Open the message
                    break;
            }
        });
    }
}
