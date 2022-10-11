using CommunityToolkit.Mvvm.Messaging;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class ToggleWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleWidget"/> class.
        /// </summary>
        public ToggleWidget()
        {
            InitializeComponent();

            Loading += ToggleWidget_Loading;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the switch is on or off.
        /// </summary>
        public string State
        {
            get => Widget.Value;
            set
            {
                if (value.CompareTo(Widget.Value) == 0)
                {
                    return;
                }

                Widget.Value = value;
            }
        }

        private void ToggleWidget_Loading(FrameworkElement sender, object args)
        {
            SetState();
        }

        internal override void SetState()
        {
            if (string.IsNullOrEmpty(Widget?.Item?.State) ||
                string.CompareOrdinal(Widget?.Item?.State, "NULL") == 0)
            {
                Widget.Value = OpenHABCommands.OffCommand;
            }
            else if (Widget.Value == null)
            {
                Widget.Value = Widget.Item.State;
            }
            else if (Widget.Item.State != Widget.Value)
            {
                Widget.Value = Widget.Item.State;
            }

            RaisePropertyChanged(nameof(Widget));
            RaisePropertyChanged(nameof(State));
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch == null)
            {
                return;
            }

            string currentValue = toggleSwitch.IsOn ? OpenHABCommands.OnCommand : OpenHABCommands.OffCommand;
            if (string.CompareOrdinal(currentValue, Widget?.Item?.State) != 0)
            {
                Widget.Item.State = currentValue;
                StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, currentValue));
            }
        }
    }
}