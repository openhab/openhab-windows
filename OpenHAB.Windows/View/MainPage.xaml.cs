using System;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;
using OpenHAB.Core.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// Startup page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _errorMessageTimer;

        /// <summary>
        /// Gets the datacontext, for use in compiled bindings
        /// </summary>
        public MainViewModel Vm => DataContext as MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            Messenger.Default.Register<FireErrorMessage>(this, msg => ShowErrorMessage());

            SetupErrorTimer();

            Vm.CurrentWidgets.CollectionChanged += (sender, args) =>
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = WidgetNavigationService.CanGoBack
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) => Vm.WidgetGoBack();
        }

        private void SetupErrorTimer()
        {
            _errorMessageTimer = new DispatcherTimer();
            _errorMessageTimer.Interval = TimeSpan.FromSeconds(5);
            _errorMessageTimer.Tick += ErrorMessageTimerOnTick;
        }

        private void ErrorMessageTimerOnTick(object sender, object o)
        {
            _errorMessageTimer.Stop();
            ErrorGoneStoryboard.Begin();
        }

        private void ShowErrorMessage()
        {
            ErrorMessageStoryboard.Begin();
            _errorMessageTimer.Start();
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }
    }
}
