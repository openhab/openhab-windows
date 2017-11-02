using System;
using Windows.UI.Xaml.Input;

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
        }

        private async void ImageWidget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await PopupDialog.ShowAsync();
        }
    }
}