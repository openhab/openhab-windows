using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using openHAB.Core;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Windows.Services;
using openHAB.Windows.ViewModel;
using System;
using System.Threading.Tasks;

namespace openHAB.Windows.View
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
            DataContext = DIService.Instance.GetService<MainViewModel>();
            _logger = DIService.Instance.GetService<ILogger<MainPage>>();

            InitializeComponent();

            //Vm.CurrentWidgets.CollectionChanged += async (sender, args) =>
            //{
            //    DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            //    await dispatcherQueue.EnqueueAsync(() =>
            //     {
            //         SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = WidgetNavigationService.CanGoBack
            //         ? AppViewBackButtonVisibility.Visible
            //         : AppViewBackButtonVisibility.Collapsed;
            //     });
            //};

            //SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) => Vm.WidgetGoBack();
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StrongReferenceMessenger.Default.Register<FireErrorMessage>(this, async (recipient, msg) => await ShowErrorMessage(recipient, msg));
            StrongReferenceMessenger.Default.Register<FireInfoMessage>(this, async (recipient, msg) => await ShowInfoMessage(recipient, msg));

            await Vm.LoadSitemapsAndItemData().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StrongReferenceMessenger.Default.Unregister<FireErrorMessage>(this);
            StrongReferenceMessenger.Default.Unregister<FireInfoMessage>(this);

            ErrorNotification.IsOpen = false;
            InfoNotification.IsOpen = false;
        }

#pragma warning disable S1172 // Unused method parameters should be removed

        private async Task ShowErrorMessage(object recipient, FireErrorMessage message)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            try
            {
                string errorMessage = null;
                if (message == null || string.IsNullOrEmpty(message.ErrorMessage))
                {
                    errorMessage = AppResources.Values.GetString("MessageError");
                    ErrorNotification.Message = errorMessage;
                    ErrorNotification.IsOpen = true;
                }
                else
                {
                    errorMessage = message.ErrorMessage;
                }

                await App.DispatcherQueue.EnqueueAsync(() =>
                {
                    ErrorNotification.Message = errorMessage;
                    ErrorNotification.IsOpen = true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show error message failed.");
            }
        }

#pragma warning disable S1172 // Unused method parameters should be removed

        private async Task ShowInfoMessage(object recipient, FireInfoMessage msg)
#pragma warning restore S1172 // Unused method parameters should be removed
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

                await App.DispatcherQueue.EnqueueAsync(() =>
                {
                    InfoNotification.Message = message;
                    InfoNotification.IsOpen = true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Show info message failed.");
            }
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }

        private void SitemapNavigation_SelectionChanged(
            Microsoft.UI.Xaml.Controls.NavigationView sender,
            Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                Frame.Navigate(typeof(SettingsPage));
            }
        }

        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            OpenHABWidget widget = args.Item as OpenHABWidget;
            this.Vm.SelectedSitemap.WidgetGoBack(widget);
        }
    }
}