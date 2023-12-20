using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using openHAB.Windows.View;
using System;

namespace openHAB.Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private ILogger<App> _logger;
        private ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;

            DispatcherQueue = DispatcherQueue.GetForCurrentThread();

            _logger = DIService.Instance.GetService<ILogger<App>>();
            _settingsService = DIService.Instance.GetService<ISettingsService>();

            INotificationManager notificationManager = DIService.Instance.GetService<INotificationManager>();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // For more details, see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs.
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs e)
        {
            _logger.LogInformation("=== Start Application ===");
            _settingsService.SetProgramLanguage(null);

            // TODO This code defaults the app to a single instance app. If you need multi instance app, remove this part.
            // Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/applifecycle#single-instancing-in-applicationonlaunched
            // If this is the first instance launched, then register it as the "main" instance.
            // If this isn't the first instance launched, then "main" will already be registered,
            // so retrieve it.
            AppInstance mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");
            AppActivationArguments activatedEventArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

            // If the instance that's executing the OnLaunched handler right now
            // isn't the "main" instance.
            if (!mainInstance.IsCurrent)
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            // TODO This code handles app activation types. Add any other activation kinds you want to handle.
            // Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/applifecycle#file-type-association
            if (activatedEventArgs.Kind == ExtendedActivationKind.File)
            {
                OnFileActivated(activatedEventArgs);
            }

            // Initialize MainWindow here
            MainWindow = new MainWindow();

            Frame rootFrame = MainWindow.Content as Frame;
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // Place the frame in the current Window
                MainWindow.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            MainWindow.Activate();
        }

        // TODO This is an example method for the case when app is activated through a file.
        // Feel free to remove this if you do not need this.
        public void OnFileActivated(AppActivationArguments activatedEventArgs)
        {
        }

        public static DispatcherQueue DispatcherQueue { get; private set; }

        public static Window MainWindow { get; private set; }
    }
}
