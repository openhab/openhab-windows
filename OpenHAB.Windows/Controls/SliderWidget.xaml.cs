using System;
using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB slider
    /// </summary>
    public sealed partial class SliderWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderWidget"/> class.
        /// </summary>
        public SliderWidget()
        {
            InitializeComponent();
        }

        private void RadialSlider_OnValueChanged(object sender, EventArgs e)
        {
            if (Widget == null)
            {
                return;
            }

            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, ((RadialSlider)sender)?.Value.ToString(CultureInfo.InvariantCulture)));
            RaisePropertyChanged(nameof(Widget));
        }

        private void Widget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (Widget.Item.State == "0")
            {
                Widget.Item.State = "100";
                Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, "100"));
            }
            else
            {
                Widget.Item.State = "0";
                Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, "0"));
            }
        }

        internal override void SetState()
        {
        }
    }
}
