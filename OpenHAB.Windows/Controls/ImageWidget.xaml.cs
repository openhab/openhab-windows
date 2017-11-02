using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

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
            SetSources();
        }

        private void SetSources()
        {
            ThumbImage.Source = new BitmapImage(
                    new Uri(Widget.Url, UriKind.Absolute))
                { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

            FullImage.Source = new BitmapImage(
                    new Uri(Widget.Url, UriKind.Absolute))
                { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        }

        private async void ImageWidget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await PopupDialog.ShowAsync();
        }
    }
}