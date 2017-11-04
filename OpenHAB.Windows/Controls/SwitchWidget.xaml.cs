using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SwitchWidget : WidgetBase
    {
        private bool _isOn;

        /// <summary>
        /// Gets or sets a value indicating whether the switch is on or off
        /// </summary>
        public bool IsOn
        {
            get => _isOn;

            set
            {
                if (_isOn == value)
                {
                    return;
                }

                _isOn = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchWidget"/> class.
        /// </summary>
        public SwitchWidget()
        {
            InitializeComponent();
            Loaded += SwitchWidget_Loaded;
        }

        private void SwitchWidget_Loaded(object sender, RoutedEventArgs e)
        {
            SetState();
        }

        internal override void SetState()
        {
            // Fix for slider things with a switch item
            if (Widget.Item.State != "ON" && Widget.Item.State != "OFF" && int.TryParse(Widget.Item.State, out int state))
            {
                IsOn = state > 0;
            }
            else
            {
                IsOn = Widget.Item.State == "ON";
            }

            VisualStateManager.GoToState(this, IsOn ? "OnState" : "OffState", false);
        }

        private void OnToggle()
        {
            IsOn = !IsOn;
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, IsOn ? OpenHABCommands.OnCommand : OpenHABCommands.OffCommand));
        }

        private void OnToggle(object sender, TappedRoutedEventArgs e)
        {
            OnToggle();
            VisualStateManager.GoToState(this, ToggleStates.CurrentState == OnState ? "OffState" : "OnState", true);
        }
    }
}