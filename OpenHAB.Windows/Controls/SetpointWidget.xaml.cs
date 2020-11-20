using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;

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
            // if Widget Step == 0 --> Step = 1
            float step = Widget.Step;
            if (step == 0)
            {
                step = 1;
            }

            double value = Widget.Item.GetStateAsDoubleValue();
            value += step;

            if (value > Widget.MaxValue)
            {
                value = Widget.MaxValue;
            }

            Widget.Item.UpdateValue(value);
            RaisePropertyChanged(nameof(Widget));
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            // if Widget Step == 0 --> Step = 1
            float step = Widget.Step;
            if (step == 0)
            {
                step = 1;
            }

            double value = Widget.Item.GetStateAsDoubleValue();
            value -= step;

            if (value < Widget.MinValue)
            {
                value = Widget.MinValue;
            }

            Widget.Item.UpdateValue(value);
            RaisePropertyChanged(nameof(Widget));
        }

        internal override void SetState()
        {
            //OpenHABWidgetMapping itemState = Widget?.Mappings.FirstOrDefault(_ => _.Command == Widget.Item.State);
            //SelectionComboBox.SelectedItem = itemState;
        }

        private void SetPointWidget_Loaded(object sender, RoutedEventArgs e)
        {
            float step = 1;
            if (Widget.Step != 0)
            {
                step = Widget.Step;
            }
            for(float i = Widget.MinValue; i <= Widget.MaxValue; i += step)
            {
                comboBox.Items.Add(i.ToString());
            }
            SetState();
        }


    }
}