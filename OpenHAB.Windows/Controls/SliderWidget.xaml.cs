using System;
using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;

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
        }
    }
}
