using System;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;
using OpenHAB.Core.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// Startup page of the application.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _errorMessageTimer;

        /// <summary>
        /// Gets the datacontext, for use in compiled bindings.
        /// </summary>
        public MainViewModel Vm => DataContext as MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            Messenger.Default.Register<FireErrorMessage>(this, msg => ShowErrorMessage(msg));
            Messenger.Default.Register<FireInfoMessage>(this, msg => ShowInfoMessage(msg));

            Vm.CurrentWidgets.CollectionChanged += (sender, args) =>
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = WidgetNavigationService.CanGoBack
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) => Vm.WidgetGoBack();
        }

        /// <inheritdoc/>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await Vm.LoadData();
            base.OnNavigatedTo(e);
        }

        private void ShowErrorMessage(FireErrorMessage message)
        {
            ErrorNotification.Show(message.ErrorMessage);
        }

        private void ShowInfoMessage(FireInfoMessage msg)
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

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
            {
                InfoNotification.Show(message);
            });
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }
    }
}
