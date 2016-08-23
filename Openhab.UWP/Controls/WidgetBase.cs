using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Openhab.Model;

namespace Openhab.UWP.Controls
{
    public class WidgetBase : UserControl
    {
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register(
            nameof(Widget), typeof(OpenHABWidget), typeof(WidgetBase), new PropertyMetadata(default(OpenHABWidget)));

        public OpenHABWidget Widget
        {
            get { return (OpenHABWidget) GetValue(WidgetProperty); }
            set { SetValue(WidgetProperty, value); }
        }
    }
}
