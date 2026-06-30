using System;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using openHAB.Common;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Core.Services;
using openHAB.Windows.Messages;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.View;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SitemapPage : Microsoft.UI.Xaml.Controls.Page
{
    private readonly SitemapService _sitemapService;
    private readonly IServiceProvider _serviceProvider;
    private SitemapViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapPage" /> class
    /// .</summary>
    public SitemapPage()
    {
        this.InitializeComponent();

        StrongReferenceMessenger.Default.Register<SitemapChanged>(this, (obj, message)
                => OnSitemapChangedEvent(message));

        _sitemapService = Program.Host.Services.GetRequiredService<SitemapService>();
        _serviceProvider = Program.Host.Services.GetRequiredService<IServiceProvider>();
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

        StrongReferenceMessenger.Default.Unregister<SitemapChanged>(this);
        ViewModel?.Dispose();
    }

    protected async override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        string sitemapUrl = e.Parameter as string;
        if (string.IsNullOrEmpty(sitemapUrl))
        {
            return;
        }

        StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

        Sitemap sitemap = await _sitemapService.GetSitemapByUrlAsync(sitemapUrl);
        if (sitemap is null)
        {
            StrongReferenceMessenger.Default.Send<TriggerInfoMessage>(new TriggerInfoMessage(MessageSeverity.Error, AppResources.Values.GetString("MessagesNotReachable")));
            StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));

            return;
        }

        _viewModel = await SitemapViewModel.CreateAsync(sitemap, _sitemapService, _serviceProvider);

        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        await dispatcherQueue.EnqueueAsync(() =>
        {
            DataContext = _viewModel;
            Bindings.Update();
        });

        StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));
    }

    private void MasterListView_OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is WidgetViewModel widgetViewModel)
        {
            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(widgetViewModel));
        }
    }

    private async void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Item is WidgetViewModel widget)
        {
            await ViewModel.GoBackTo(widget);
        }
    }

    private void SitemapTextBlock_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        ViewModel?.NavigateToRoot();
    }

    private void OnSitemapChangedEvent(SitemapChanged message)
    {
        if (message.Sitemap == null)
        {
            return;
        }
    }
}
