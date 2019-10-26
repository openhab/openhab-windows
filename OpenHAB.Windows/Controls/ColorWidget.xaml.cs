using System;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Common;
using OpenHAB.Core.Messages;
using Windows.UI;
using Windows.UI.Xaml;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB text.
    /// </summary>
    public sealed partial class ColorWidget : WidgetBase
    {
        private Color _selectedColor;

        /// <summary>
        /// Gets or sets the color currently selected in the colorpicker widget.
        /// </summary>
        public Color SelectedColor
        {
            get => _selectedColor;

            set
            {
                if (_selectedColor == value)
                {
                    return;
                }

                _selectedColor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWidget"/> class.
        /// </summary>
        public ColorWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetState();
        }

        internal override void SetState()
        {
            var rgbString = Widget.Item.State.Split(',');

            if (rgbString.Length == 0)
            {
                return;
            }

            SelectedColor = Core.Common.ColorHelper.FromHSV(Convert.ToDouble(rgbString[0]), Convert.ToDouble(rgbString[1]), Convert.ToDouble(rgbString[2]));
        }

        private void ColorMap_OnColorChanged(object sender, ColorChangedEventArgs e)
        {
            if (Widget == null)
            {
                return;
            }

            var colorMap = (ColorMap)sender;
            var hsv = Core.Common.ColorHelper.ToHSV(colorMap.Color);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, $"{hsv.X},{hsv.Y},{hsv.Z}"));
        }
    }
}
