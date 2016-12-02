using Windows.UI.Core;
using Windows.UI.ViewManagement;
using OpenHAB.Core.ViewModel;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// Startup page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
            Vm.CurrentWidgets.CollectionChanged += (sender, args) =>
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = WidgetNavigationService.CanGoBack
                    ? AppViewBackButtonVisibility.Visible
                    : AppViewBackButtonVisibility.Collapsed;
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) => Vm.WidgetGoBack();
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }
    }
}
