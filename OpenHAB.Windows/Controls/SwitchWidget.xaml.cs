using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core;
using OpenHAB.Core.Messages;

namespace OpenHAB.Windows.Controls
{
    public sealed partial class SwitchWidget : WidgetBase
    {
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