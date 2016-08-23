using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Openhab.Core;
using Openhab.Model.Messages;

namespace Openhab.UWP.Controls
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