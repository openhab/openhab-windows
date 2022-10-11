using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml.Controls;

namespace openHAB.Windows.Extensions
{
    /// <summary>Extensions for ContentDialog control.</summary>
    public static class ContentDialogExtensions
    {
        /// <summary>Adjusts the ContentDialog size.</summary>
        /// <param name="dialog">The dialog.</param>
        public static void AdjustSize(this ContentDialog dialog)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            if (size.Width > 1000.0)
            {
                dialog.MinWidth = 640;
                dialog.MinHeight = 400;
            }
            else
            {
                dialog.MinWidth = dialog.MinHeight = 300;
            }
        }
    }
}