using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SwitchWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchWidget"/> class.
        /// </summary>
        public SwitchWidget()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, ((ToggleSwitch)sender).IsOn ? OpenHABCommands.OnCommand : OpenHABCommands.OffCommand));
        }
    }
}