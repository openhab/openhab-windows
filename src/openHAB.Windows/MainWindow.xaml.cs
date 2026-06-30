using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Windows.View;
using openHAB.Windows.ViewModel;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics;

namespace openHAB.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly ILogger<MainPage> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(MainViewModel mainViewModel, ILogger<MainPage> logger)
    {
        _logger = logger;

        StrongReferenceMessenger.Default.Register<TriggerInfoMessage>(this, async (recipient, msg)
           => await ShowInfoMessage(recipient, msg));

        this.InitializeComponent();

        this.ExtendsContentIntoTitleBar = true;
        this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        this.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
        AppTitleBar.Loaded += AppTitleBar_Loaded;
        AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
        TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;

        Vm = mainViewModel;
        Root.DataContext = Vm;

        _ = Vm.LoadSitemapsAndItemData();
    }

    /// <summary>
    /// Gets the root frame.
    /// </summary>
    /// <value>
    /// The root frame.
    /// </value>
    public Frame RootFrame => ContentFrame;

    /// <summary>
    /// Gets the data context, for use in compiled bindings.
    /// </summary>
    public MainViewModel Vm
    {
        get;
        private set;
    }

    private void NavigationViewItemMainUI_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        NavigateToMainUI();
    }

    /// <summary>
    /// Navigates to the main UI page and clears the current sitemap and menu selection.
    /// </summary>
    public void NavigateToMainUI()
    {
        SitemapNavigation.IsPaneOpen = false;

        Vm.SelectedSitemap = null;
        Vm.SelectedMenuItem = null;

        ContentFrame.Navigate(typeof(MainUIPage));
    }

    private void SitemapNavigation_Loaded(object sender, RoutedEventArgs e)
    {
        if (SitemapNavigation.SettingsItem is FrameworkElement settings)
        {
            settings.FlowDirection = FlowDirection.LeftToRight;
        }
    }

    private void SitemapNavigation_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
    {

        NavigationViewItem item = args.SelectedItem as NavigationViewItem;
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }
        else if (args.SelectedItem is Sitemap)
        {
            Sitemap sitemap = args.SelectedItem as Sitemap;
            if (sitemap != null)
            {
                ContentFrame.Navigate(typeof(SitemapPage), sitemap.Link);
            }
        }
    }

    private async Task ShowInfoMessage(object recipient, TriggerInfoMessage msg)
    {
        Vm.Notifications.Add(msg);

        Notification notification = new Notification()
        {
            Title = $"Notification {DateTimeOffset.Now}",
            Message = msg.Message,
            Severity = (InfoBarSeverity)msg.Severity,
        };

        NotificationQueue.Show(notification);
    }

    #region App TitleBar

    private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
    {
        SetRegionsForCustomTitleBar();
    }

    private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetRegionsForCustomTitleBar();
    }
    private RectInt32 GetRect(Rect bounds, double scale)
    {
        return new RectInt32(
            _X: (int)Math.Round(bounds.X * scale),
            _Y: (int)Math.Round(bounds.Y * scale),
            _Width: (int)Math.Round(bounds.Width * scale),
            _Height: (int)Math.Round(bounds.Height * scale)
        );
    }

    private void SetRegionsForCustomTitleBar()
    {
        // Specify the interactive regions of the title bar.

        double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

        RightPaddingColumn.Width = new GridLength(this.AppWindow.TitleBar.RightInset / scaleAdjustment);
        LeftPaddingColumn.Width = new GridLength(this.AppWindow.TitleBar.LeftInset / scaleAdjustment);

        //GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
        //Rect bounds = transform.TransformBounds(new Rect(0, 0,
        //                                                 TitleBarSearchBox.ActualWidth,
        //                                                 TitleBarSearchBox.ActualHeight));
        //RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

        //transform = PersonPic.TransformToVisual(null);
        //bounds = transform.TransformBounds(new Rect(0, 0,
        //                                            PersonPic.ActualWidth,
        //                                            PersonPic.ActualHeight));
        //RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

        var rectArray = new RectInt32[] { /*SearchBoxRect,/*, PersonPicRect*/ };

        InputNonClientPointerSource nonClientInputSrc =
            InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
    }
    #endregion

    private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (e.Content is LogViewerPage)
        {
            SitemapNavigation.SelectedItem = null;
        }
    }
}
