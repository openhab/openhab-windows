using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Services;
using OpenHAB.Windows.Controls;
using OpenHAB.Windows.Services;
using OpenHAB.Windows.ViewModel;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// In this page users can set their connection to the OpenHAB server.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private IAppManager _appManager;
        private SettingsViewModel _settingsViewModel;
        private ILogger<SettingsViewModel> _logger;
        private IAppManager _appManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();

            _settingsViewModel = (SettingsViewModel)DIService.Instance.Services.GetService(typeof(SettingsViewModel));
            DataContext = _settingsViewModel;

            _logger = (ILogger<SettingsViewModel>)DIService.Instance.Services.GetService(typeof(ILogger<SettingsViewModel>));

            SettingOptionsListView.SelectedIndex = 0;

            _appManager = (IAppManager)DIService.Instance.Services.GetService(typeof(IAppManager));
        }

        #region Page Navigation

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<SettingsUpdatedMessage>(this, msg => HandleSettingsUpdate(msg));

            var autostartEnabled = await _appManager.IsStartupEnabled().ConfigureAwait(false);
            var canAppAutostartEnabled = await _appManager.CanEnableAutostart().ConfigureAwait(false);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  _settingsViewModel.Settings.IsAppAutostartEnabled = autostartEnabled;
                  _settingsViewModel.Settings.CanAppAutostartEnabled = canAppAutostartEnabled;
              });

            AppAutostartSwitch.Toggled += AppAutostartSwitch_Toggled;
        }

        /// <inheritdoc/>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Messenger.Default.Unregister<SettingsUpdatedMessage>(this, msg => HandleSettingsUpdate(msg));
            AppAutostartSwitch.Toggled -= AppAutostartSwitch_Toggled;
        }

        #endregion

        /// <summary>
        /// Gets the datacontext, for use in compiled bindings.
        /// </summary>
        public SettingsViewModel Vm => DataContext as SettingsViewModel;

        private static ConnectionDialog CreateConnectionDialog()
        {
            ConnectionDialog connectionDialog = new ConnectionDialog();
            connectionDialog.DefaultButton = ContentDialogButton.Primary;

            return connectionDialog;
        }

        private void AppSettingsListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppSettings.Visibility = Visibility.Visible;
            ConnectionSettings.Visibility = Visibility.Collapsed;
        }

        private void ConnectionSettingsListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AppSettings.Visibility = Visibility.Collapsed;
            ConnectionSettings.Visibility = Visibility.Visible;
        }

        private async void HandleSettingsUpdate(SettingsUpdatedMessage msg)
        {
            try
            {
                if (msg.IsSettingsValid && msg.SettingsPersisted)
                {
                    Frame.BackStack.Clear();
                    Frame.Navigate(typeof(MainPage));

                    return;
                }
                else if (!msg.IsSettingsValid)
                {
                    string message = AppResources.Values.GetString("MessageSettingsConnectionConfigInvalid");
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        SettingsNotification.Message = message;
                        SettingsNotification.IsOpen = true;
                    });
                }
                else
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        SettingsNotification.IsOpen = false;
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show info message failed.");
            }
        }

        private void OpenLocalConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectionDialog connectionDialog = CreateConnectionDialog();
            connectionDialog.DataContext = Vm.Settings.LocalConnection;

            connectionDialog.ShowAsync();
        }

        private void OpenRemoteConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectionDialog connectionDialog = CreateConnectionDialog();
            connectionDialog.DataContext = Vm.Settings.RemoteConnection;

            connectionDialog.ShowAsync();
        }

        private async void AppAutostartSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            _appManager.ToggleAutostart();

            var autostartEnabled = await _appManager.IsStartupEnabled().ConfigureAwait(false);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _settingsViewModel.Settings.IsAppAutostartEnabled = autostartEnabled;
            });
        }
    }
}