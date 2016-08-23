using System.Globalization;
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using Openhab.Model.Messages;

namespace Openhab.UWP.Controls
{
    public sealed partial class SliderWidget : WidgetBase
    {
        public SliderWidget()
        {
            InitializeComponent();
        }

        private void Slider_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, e.NewValue.ToString(CultureInfo.InvariantCulture)));
        }
    }
}
