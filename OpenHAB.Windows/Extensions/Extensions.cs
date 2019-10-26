using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Extensions
{
    public static class ContentDialogExtensions
    {
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
                dialog.MinWidth = dialog.MinHeight = 300;
        }
    }
}