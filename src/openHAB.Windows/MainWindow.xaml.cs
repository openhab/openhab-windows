using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using openHAB.Common;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Windows.Messages;
using openHAB.Windows.Services;
using openHAB.Windows.View;
using openHAB.Windows.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics;

namespace openHAB.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly ILogger<MainPage> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            _logger = DIService.Instance.GetService<ILogger<MainPage>>();

            this.InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            this.AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
            AppTitleBar.Loaded += AppTitleBar_Loaded;
            AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
            TitleBarTextBlock.Text = AppInfo.Current.DisplayInfo.DisplayName;

            Vm = DIService.Instance.GetService<MainViewModel>();
            Root.DataContext = Vm;

            StrongReferenceMessenger.Default.Register<ConnectionErrorMessage>(this, async (recipient, msg) => await ShowErrorMessage(recipient, msg));
            StrongReferenceMessenger.Default.Register<FireInfoMessage>(this, async (recipient, msg) => await ShowInfoMessage(recipient, msg));

            Vm.LoadSitemapsAndItemData();
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

        private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
        {
            WidgetViewModel widget = args.Item as WidgetViewModel;
            if (widget == null)
            {
                _logger.LogWarning("Breadcrumb item is not a widget.");
                return;
            }

            StrongReferenceMessenger.Default.Send(new WidgetNavigationMessage(null, widget, EventTriggerSource.Breadcrumb), Vm.SelectedSitemap.Name);
        }

        private async Task ShowErrorMessage(object recipient, ConnectionErrorMessage message)
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

        private async Task ShowInfoMessage(object recipient, FireInfoMessage msg)
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

        private void SitemapNavigation_SelectionChanged(
            NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                sender.AlwaysShowHeader = false;
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                sender.AlwaysShowHeader = true;
                Sitemap sitemap = args.SelectedItem as Sitemap;
                if (sitemap != null)
                {
                    ContentFrame.Navigate(typeof(SitemapPage), sitemap.Link);
                }
            }
        }

        private void SitemapTextBlock_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new WidgetNavigationMessage(null, null, EventTriggerSource.Root), Vm.SelectedSitemap.Name);
        }

        #region App TitleBar

        private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetRegionsForCustomTitleBar();
        }

        private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            SetRegionsForCustomTitleBar();
        }

        private void SetRegionsForCustomTitleBar()
        {
            // Specify the interactive regions of the title bar.

            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;

            RightPaddingColumn.Width = new GridLength(this.AppWindow.TitleBar.RightInset / scaleAdjustment);
            LeftPaddingColumn.Width = new GridLength(this.AppWindow.TitleBar.LeftInset / scaleAdjustment);

            GeneralTransform transform = TitleBarSearchBox.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0,
                                                             TitleBarSearchBox.ActualWidth,
                                                             TitleBarSearchBox.ActualHeight));
            RectInt32 SearchBoxRect = GetRect(bounds, scaleAdjustment);

            //transform = PersonPic.TransformToVisual(null);
            //bounds = transform.TransformBounds(new Rect(0, 0,
            //                                            PersonPic.ActualWidth,
            //                                            PersonPic.ActualHeight));
            //RectInt32 PersonPicRect = GetRect(bounds, scaleAdjustment);

            var rectArray = new RectInt32[] { SearchBoxRect/*, PersonPicRect*/ };

            InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
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

        #endregion
    }
}
