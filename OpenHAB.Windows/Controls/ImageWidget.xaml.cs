using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Imaging;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB Image
    /// </summary>
    public sealed partial class ImageWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageWidget"/> class.
        /// </summary>
        public ImageWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetState();
        }

        internal override async void SetState()
        {
            if (Widget.Item != null && Widget.Item.State.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                await SetLocalData();
            }
            else
            {
                SetUrl();
            }
        }

        private async Task SetLocalData()
        {
            var image = new BitmapImage();
            string data = Widget.Item.State.Substring(Widget.Item.State.IndexOf(',') + 1);
            await image.LoadFromBase64StringAsync(data);

            ThumbImage.Source = image;
            FullImage.Source = image;
        }

        private void SetUrl()
        {
            string url;
            if (Widget.Item != null && Widget.Item.State.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                url = Widget.Item.State;
            }
            else
            {
                url = Widget.Url;
            }

            ThumbImage.Source = new BitmapImage(
                    new Uri(url, UriKind.Absolute))
                { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

            FullImage.Source = new BitmapImage(
                    new Uri(url, UriKind.Absolute))
                { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        }

        private async void ImageWidget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (Widget.LinkedPage != null)
            {
                return;
            }

            await PopupDialog.ShowAsync();
        }
    }
}