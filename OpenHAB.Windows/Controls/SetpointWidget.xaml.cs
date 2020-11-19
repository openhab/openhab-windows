using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using Windows.UI.Xaml;
using System.Text.RegularExpressions;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SetpointWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetpointWidget"/> class.
        /// </summary>
        public SetpointWidget()
        {
            InitializeComponent();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            double value = GetDoubleValue(Widget.Item.State);
            value += Widget.Step;

            //if Widget Step == 0 --> Step = 1

            if (value > Widget.MaxValue)
            {
                value = Widget.MaxValue;
            }

            Widget.Item.State = value.ToString(CultureInfo.CurrentCulture);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, value.ToString(CultureInfo.InvariantCulture)));
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            double value = GetDoubleValue(Widget.Item.State);
            value -= Widget.Step;

            if (value < Widget.MinValue)
            {
                value = Widget.MinValue;
            }

            Widget.Item.State = value.ToString(CultureInfo.CurrentCulture);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, value.ToString(CultureInfo.InvariantCulture)));
        }

        private double GetDoubleValue(string state)
        {
            string newstate = Regex.Replace(state, "[^0-9,.]", string.Empty);
            double value = 0;
            _ = double.TryParse(newstate, out value);

            return value;
        }

        internal override void SetState()
        {
        }
    }
}