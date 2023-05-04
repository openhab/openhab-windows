using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using openHAB.Core.Messages;
using openHAB.Core.Model;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class RollershutterWidget : WidgetBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether the switch is on or off.
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RollershutterWidget"/> class.
        /// </summary>
        public RollershutterWidget()
        {
            InitializeComponent();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, OpenHABCommands.UpCommand));
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, OpenHABCommands.StopCommand));
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, OpenHABCommands.DownCommand));
        }

        internal override void SetState()
        {
        }
    }
}