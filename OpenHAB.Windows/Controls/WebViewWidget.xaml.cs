using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHab WebView
    /// </summary>
    public sealed partial class WebViewWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebViewWidget"/> class.
        /// </summary>
        public WebViewWidget()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetState();
        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await PopupDialog.ShowAsync();
        }

        internal override void SetState()
        {
            if (!string.IsNullOrEmpty(Widget?.Url))
            {
                WebView.Source = WebViewFull.Source = new Uri(Widget.Url);
            }
        }
    }
}
