using System;
using System.Globalization;
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
        private double _brightness;
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWidget"/> class.
        /// </summary>
        public ColorWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Gets or sets the color currently selected in the color picker widget.
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
                var hsvclr = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToHsv(_selectedColor);
                Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, $"{hsvclr.H.ToString(CultureInfo.InvariantCulture)},{(hsvclr.S*100).ToString(CultureInfo.InvariantCulture)}, {((hsvclr.V)*100).ToString(CultureInfo.InvariantCulture)}"));
                RaisePropertyChanged();
            }
        }


        internal override void SetState()
        {
            var rgbString = Widget.Item?.State.Split(',');

            if (rgbString == null || rgbString.Length == 0)
            {
                return;
            }

            //SelectedColor = Core.Common.ColorHelper.FromHSV(Convert.ToDouble(rgbString[0]), Convert.ToDouble(rgbString[1]), Convert.ToDouble(rgbString[2]));
        }

        private void ColorMap_OnColorChanged(object sender, ColorChangedEventArgs e)
        {
            if (Widget == null)
            {
                return;
            }

            var colorMap = (ColorMap)sender;
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, $"{colorMap.Angle.ToString(CultureInfo.InvariantCulture)},{colorMap.RadialPos.ToString(CultureInfo.InvariantCulture)}, 100"));
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetState();
        }
    }
}
