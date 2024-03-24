using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using openHAB.Core.Client.Messages;
using openHAB.Core.Model;
using openHAB.Windows.ViewModel;

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

        ///// <summary>
        ///// A bindable property to bind the State to the control.
        ///// </summary>
        //public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        //    nameof(State), typeof(string), typeof(ToggleWidget), new PropertyMetadata(default(WidgetViewModel), StatePropertyChangedCallback));

        //private static void StatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{

        //}

        /// <summary>
        /// Gets or sets a value indicating whether the switch is on or off.
        /// </summary>
        public string State
        {
            get => Widget.Item.State;
            set
            {
                if (value.CompareTo(Widget.Item.State) == 0)
                {
                    return;
                }

                Widget.Item.State = value;
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
                Widget.Item.State = OpenHABCommands.OffCommand;
            }
            else if (Widget.Item.State == null)
            {
                Widget.Item.State = Widget.Item.State;
            }
            else if (Widget.Item.State != Widget.Item.State)
            {
                Widget.Item.State = Widget.Item.State;
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