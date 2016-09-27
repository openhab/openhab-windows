using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    public sealed partial class SliderWidget : WidgetBase
    {
        public SliderWidget()
        {
            InitializeComponent();
        }

        private void Slider_OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, ((Slider)sender)?.Value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
