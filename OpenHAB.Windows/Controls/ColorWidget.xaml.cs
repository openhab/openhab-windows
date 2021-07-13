using System;
using System.Globalization;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Messages;
using OpenHAB.Windows.Services;
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
        private ILogger<ColorWidget> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorWidget"/> class.
        /// </summary>
        public ColorWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            _logger = (ILogger<ColorWidget>)DIService.Instance.Services.GetService(typeof(ILogger<ColorWidget>));
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
                RaisePropertyChanged();
            }
        }

        internal override void SetState()
        {
            string rgbString = Widget.Item?.State;
            string[] rgbSegements = Widget.Item?.State.Split(',');

            Regex rgbRegEx = new Regex(@"(\d{1,3}),(\d{1,3}),(\d{1,3})");

            if (rgbString == null || rgbString.Length == 0 || !rgbRegEx.IsMatch(rgbString))
            {
                _logger.LogWarning($"Item state '{rgbString}' is not a valid RGB value");
                return;
            }

            double h = Convert.ToDouble(rgbSegements[0], CultureInfo.InvariantCulture);
            double s = Convert.ToDouble(rgbSegements[1], CultureInfo.InvariantCulture) / 100;
            double v = Convert.ToDouble(rgbSegements[2], CultureInfo.InvariantCulture);

            // Disable Changed Events
            ClrPicker.ColorChanged -= ClrPicker_ColorChanged;
            BrightnessSlider.ValueChanged -= BrightnessSlider_ValueChanged;

            // Check if brightness > 0 to prevent Widget from losing last Color
            if (v > 0)
            {
                // Set Colorproperty
                SelectedColor = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.FromHsv(h, s, 1);
            }

            // Set Brightness to Slider
            BrightnessSlider.Value = v;
            BrightnessSlider.ValueChanged += BrightnessSlider_ValueChanged;
            ClrPicker.ColorChanged += ClrPicker_ColorChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SelectedColor = Colors.White;
            SetState();
        }

        private void ClrPicker_ColorChanged(global::Windows.UI.Xaml.Controls.ColorPicker sender, global::Windows.UI.Xaml.Controls.ColorChangedEventArgs args)
        {
            ColorChanged();
        }

        private void BrightnessSlider_ValueChanged(object sender, global::Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            ColorChanged();
        }

        /// <summary>Sends the Color to Openhab Messenger.</summary>
        private void ColorChanged()
        {
            var hsvclr = Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToHsv(ClrPicker.Color);
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, $"{hsvclr.H.ToString(CultureInfo.InvariantCulture)},{(hsvclr.S * 100).ToString(CultureInfo.InvariantCulture)}, {BrightnessSlider.Value.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}
