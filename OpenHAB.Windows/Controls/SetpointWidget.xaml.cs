using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SetpointWidget : WidgetBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the switch is on or off
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetpointWidget"/> class.
        /// </summary>
        public SetpointWidget()
        {
            InitializeComponent();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            double value = double.Parse(Widget.Item.State);
            value += Widget.Step;

            if (value > Widget.MaxValue)
            {
                value = Widget.MaxValue;
            }

            Widget.Item.State = value.ToString(CultureInfo.CurrentCulture);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, value.ToString(CultureInfo.InvariantCulture)));
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            double value = double.Parse(Widget.Item.State);
            value -= Widget.Step;

            if (value < Widget.MinValue)
            {
                value = Widget.MinValue;
            }

            Widget.Item.State = value.ToString(CultureInfo.CurrentCulture);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, value.ToString(CultureInfo.InvariantCulture)));
        }
    }
}