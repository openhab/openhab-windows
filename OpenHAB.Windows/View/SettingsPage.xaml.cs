using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Services;
using OpenHAB.Core.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// In this page users can set their connection to the OpenHAB server.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private ILogger<SettingsViewModel> _logger;

        /// <summary>
        /// Gets the datacontext, for use in compiled bindings.
        /// </summary>
        public SettingsViewModel Vm => DataContext as SettingsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();

            DataContext = (SettingsViewModel)DIService.Instance.Services.GetService(typeof(SettingsViewModel));
            _logger = (ILogger<SettingsViewModel>)DIService.Instance.Services.GetService(typeof(ILogger<SettingsViewModel>));

            Messenger.Default.Register<SettingsUpdatedMessage>(this, msg => ShowInfoMessage(msg));
        }

        private async void ShowInfoMessage(SettingsUpdatedMessage msg)
        {

            try
            {
                string message = AppResources.Values.GetString("MessageSettingsConnectionConfigInvalid");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    SettingsNotification.Show(message, 30000);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show info message failed.");
            }
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}