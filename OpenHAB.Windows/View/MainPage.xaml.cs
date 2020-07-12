using System;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;
using OpenHAB.Windows.Services;
using OpenHAB.Windows.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// Startup page of the application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ILogger<MainPage> _logger;

        /// <summary>
        /// Gets the datacontext, for use in compiled bindings.
        /// </summary>
        public MainViewModel Vm => DataContext as MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            DataContext = (MainViewModel)DIService.Instance.Services.GetService(typeof(MainViewModel));
            _logger = (ILogger<MainPage>)DIService.Instance.Services.GetService(typeof(ILogger<MainPage>));

            InitializeComponent();

            Vm.CurrentWidgets.CollectionChanged += async (sender, args) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = WidgetNavigationService.CanGoBack
                     ? AppViewBackButtonVisibility.Visible
                     : AppViewBackButtonVisibility.Collapsed;
                 });
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) => Vm.WidgetGoBack();
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Register<FireErrorMessage>(this, msg => ShowErrorMessage(msg));
            Messenger.Default.Register<FireInfoMessage>(this, msg => ShowInfoMessage(msg));

            await Vm.LoadSitemapsAndItemData().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<FireErrorMessage>(this, msg => ShowErrorMessage(msg));
            Messenger.Default.Unregister<FireInfoMessage>(this, msg => ShowInfoMessage(msg));

            ErrorNotification.Dismiss();
            InfoNotification.Dismiss();

            base.OnNavigatedFrom(e);
        }

        private async void ShowErrorMessage(FireErrorMessage message)
        {
            try
            {
                string errorMessage = null;
                if (message == null || string.IsNullOrEmpty(message.ErrorMessage))
                {
                    errorMessage = AppResources.Values.GetString("MessageError");
                    ErrorNotification.Show(errorMessage);
                }
                else
                {
                    errorMessage = message.ErrorMessage;
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ErrorNotification.Show(errorMessage);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show error message failed.");
            }
        }

        private async void ShowInfoMessage(FireInfoMessage msg)
        {
            try
            {
                string message = null;
                switch (msg.MessageType)
                {
                    case MessageType.NotConfigured:
                        message = AppResources.Values.GetString("MessageNotConfigured");
                        break;
                    case MessageType.NotReachable:
                        message = AppResources.Values.GetString("MessagesNotReachable");
                        break;
                    default:
                        message = "Message not defined";
                        break;
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                  {
                      InfoNotification.Show(message, 30000);
                  });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show info message failed.");
            }
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
