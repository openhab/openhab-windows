using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using openHAB.Windows.View;

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

        public static DispatcherQueue DispatcherQueue { get; private set; }

        public static Window MainWindow { get; private set; }

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

            DispatcherQueue = global::Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            // Register for toast activation. Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

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
            MainWindow = new MainWindow();
            //Frame rootFrame = MainWindow.RootFrame;

            //if (rootFrame == null)
            //{
            //    // Create a Frame to act as the navigation context and navigate to the first page
            //    rootFrame = new Frame();

            //    // Place the frame in the current Window
            //    MainWindow.Content = rootFrame;
            //}

            //if (rootFrame.Content == null)
            //{
            //    rootFrame.Navigate(typeof(MainPage), e.Arguments);
            //}

            MainWindow.Activate();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logger.LogCritical(e.Exception, "Unhandled Exception");
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // Use the dispatcher from the window if present, otherwise the app dispatcher
            var dispatcherQueue = MainWindow?.DispatcherQueue ?? App.DispatcherQueue;

            dispatcherQueue.TryEnqueue(delegate
            {
                var args = ToastArguments.Parse(e.Argument);

                switch (args["action"])
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

                        string itemName = args["item"];
                        // Launch/bring window to foreground
                        //LaunchAndBringToForegroundIfNeeded();

                        // TODO: Open the message
                        break;
                }
            });
        }
    }
}
