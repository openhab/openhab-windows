using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using System;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Linq;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class SetpointWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetpointWidget"/> class.
        /// </summary>

        private float step; // represents the stepwidth of the item

        public SetpointWidget()
        {
            InitializeComponent();
        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {

            double value = Widget.Item.GetStateAsDoubleValue();
            value += step;

            if (value > Widget.MaxValue)
            {
                value = Widget.MaxValue;
            }

            Widget.Item.UpdateValue(value);
            RaisePropertyChanged(nameof(Widget));
            SetState();
        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            double value = Widget.Item.GetStateAsDoubleValue();
            value -= step;

            if (value < Widget.MinValue)
            {
                value = Widget.MinValue;
            }

            Widget.Item.UpdateValue(value);
            RaisePropertyChanged(nameof(Widget));
            SetState();
        }

        internal override void SetState()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                UpdateComboBox();
                comboBox.SelectedItem = Widget.Item.GetStateAsDoubleValue().ToString(CultureInfo.InvariantCulture) + Widget.Item.Unit;
                comboBox.SelectionChanged += ComboBox_SelectionChanged;
            });

        }

        private void SetPointWidget_Loaded(object sender, RoutedEventArgs e)
        {
            // if Widget Step == 0 --> Step = 1
            step = Widget.Step;
            if (step == 0)
            {
                step = 1;
            }

            SetState();
        }

        private void UpdateComboBox()
        {
            comboBox.Items.Clear();
            float step = 1;
            double currentValue = Widget.Item.GetStateAsDoubleValue();
            if (Widget.Step != 0)
            {
                step = Widget.Step;
            }
            for (float i = Widget.MinValue; i <= Widget.MaxValue; i += step)
            {
                comboBox.Items.Add(i.ToString(CultureInfo.InvariantCulture) + Widget.Item.Unit);
                if (i < currentValue &&  currentValue < (i + step))
                {
                    comboBox.Items.Add(currentValue.ToString(CultureInfo.InvariantCulture) + Widget.Item.Unit);
                }
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string newValue = (string)e.AddedItems.FirstOrDefault();
            if (Widget.Item.Unit != null)
            {
                newValue = newValue.Replace(Widget.Item.Unit, string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }

            if (newValue != null)
            {
                Widget.Item.UpdateValue(newValue);
            }
        }
    }
}