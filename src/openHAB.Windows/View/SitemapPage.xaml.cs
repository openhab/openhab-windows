using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Core.Services;
using openHAB.Windows.Services;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SitemapPage : Page
    {
        private SitemapService _sitemapService;
        private SitemapViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapPage" /> class
        /// .</summary>
        public SitemapPage()
        {
            this.InitializeComponent();

            StrongReferenceMessenger.Default.Register<SitemapChanged>(this, (obj, message)
                    => OnSitemapChangedEvent(message));

            _sitemapService = DIService.Instance.GetService<SitemapService>();
        }

        /// <summary>
        /// Gets the data context, for use in compiled bindings.
        /// </summary>
        public SitemapViewModel ViewModel
        {
            get => _viewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string sitemapUrl = e.Parameter as string;
            if (string.IsNullOrEmpty(sitemapUrl))
            {
                return;
            }

            OpenHABSitemap sitemap = await _sitemapService.GetSitemapByUrlAsync(sitemapUrl);
            _viewModel = new SitemapViewModel(sitemap);

            DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            await dispatcherQueue.EnqueueAsync(async () =>
            {
                DataContext = _viewModel;
                WidgetNavigationService.ClearWidgetNavigation();
            });
        }

        private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }

        private void OnSitemapChangedEvent(SitemapChanged message)
        {
            if(message.Sitemap == null)
            {
                return;
            }
        }
    }
}
