using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Core.Services;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SitemapPage : Page
    {
        public SitemapPage()
        {
            this.InitializeComponent();

            StrongReferenceMessenger.Default.Register<SitemapChanged>(this, (obj, message)
                    => OnSitemapChangedEvent(message));
        }

        private void OnSitemapChangedEvent(SitemapChanged message)
        {
            if(message.Sitemap == null)
            {
                return;
            }

            DataContext = new SitemapViewModel(message.Sitemap);

            if (ViewModel != null)
            {
                DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                dispatcherQueue.EnqueueAsync(() =>
                {
                    ViewModel.LoadWidgets();
                    WidgetNavigationService.ClearWidgetNavigation();
                });
            }
        }

        /// <summary>
        /// Gets the data context, for use in compiled bindings.
        /// </summary>
        public SitemapViewModel ViewModel
        {
            get => DataContext as SitemapViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }
    }
}
