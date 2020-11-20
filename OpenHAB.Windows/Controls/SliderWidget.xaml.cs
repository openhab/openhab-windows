using System;
using System.Globalization;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB slider
    /// </summary>
    public sealed partial class SliderWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderWidget"/> class.
        /// </summary>
        public SliderWidget()
        {
            InitializeComponent();
            
        }

        private void RadialSlider_OnValueChanged(object sender, EventArgs e)
        {
            if (Widget == null)
            {
                return;
            }

            Widget.Item.UpdateValue(((RadialSlider)sender)?.Value);
            RaisePropertyChanged(nameof(Widget));
        }

        private void Widget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (Widget.Item.GetStateAsDoubleValue() <= Widget.MinValue)
            {
                Widget.Item.UpdateValue(Widget.MaxValue);
            }
            else
            {
                Widget.Item.UpdateValue(Widget.MinValue);
            }
            RaisePropertyChanged(nameof(Widget));
        }

        internal override void SetState()
        {
        }

        private void WidgetBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (Widget.MaxValue != 0)
            {
                radialSlider.Maximum = Widget.MaxValue;
            }

            if (Widget.MinValue != 0)
            {
                radialSlider.Minimum = Widget.MinValue;
            }
        }
    }
}
