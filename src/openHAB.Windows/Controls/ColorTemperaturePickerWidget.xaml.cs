using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace openHAB.Windows.Controls;

/// <summary>
/// Widget control that represents an OpenHAB color temperature picker: a warm-to-cool gradient slider.
/// </summary>
public sealed partial class ColorTemperaturePickerWidget : WidgetBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColorTemperaturePickerWidget"/> class.
    /// </summary>
    public ColorTemperaturePickerWidget()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ConfigureRange();
        SetState();
    }

    private void ConfigureRange()
    {
        // Percent-based color temperature items (Dimmer) use 0..100; absolute Number:Temperature
        // items use a Kelvin range. The sitemap min/max/step override these defaults when set.
        bool isPercent = Widget.Item?.Type != null && Widget.Item.Type.Contains("Dimmer", StringComparison.OrdinalIgnoreCase);

        TempSlider.Minimum = Widget.MinValue != 0 ? Widget.MinValue : (isPercent ? 0 : 1000);
        TempSlider.Maximum = Widget.MaxValue != 0 ? Widget.MaxValue : (isPercent ? 100 : 10000);
        TempSlider.StepFrequency = Widget.Step > 0 ? Widget.Step : (isPercent ? 1 : 100);
    }

    internal override void SetState()
    {
        if (Widget?.Item == null)
        {
            return;
        }

        double value = Math.Clamp(Widget.Item.GetStateAsDoubleValue(), TempSlider.Minimum, TempSlider.Maximum);

        TempSlider.ValueChanged -= TempSlider_ValueChanged;
        TempSlider.Value = value;
        TempSlider.ValueChanged += TempSlider_ValueChanged;

        UpdateValueText();
    }

    private void UpdateValueText()
    {
        ValueText.Text = $"{(int)TempSlider.Value} {Widget?.Item?.Unit}".Trim();
    }

    private void TempSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        // ponytail: drag updates only the label; the command is committed once on release
        // (PointerCaptureLost) or KeyUp, so a slow drag does not POST per step.
        UpdateValueText();
    }

    private void TempSlider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
    {
        Commit();
    }

    private void TempSlider_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        Commit();
    }

    private void Commit()
    {
        Widget?.Item?.UpdateValue((int)TempSlider.Value);
    }
}
