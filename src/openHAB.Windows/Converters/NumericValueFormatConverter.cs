using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Data;

namespace openHAB.Windows.Converters;

/// <summary>
/// Formats a widget value so that numeric content (for example temperatures) is rendered with a
/// limited number of decimal places instead of full floating-point precision.
/// For example, "21.799999237060547 °C" is rendered as "21.8 °C".
/// </summary>
/// <remarks>
/// openHAB item states such as <c>Number:Temperature</c> are transported as full-precision values.
/// When the server-side state-description pattern is missing, those raw values reach the UI and show
/// far too many digits after the decimal separator. This converter normalizes the leading number
/// while preserving any trailing unit text.
/// </remarks>
public class NumericValueFormatConverter : IValueConverter
{
    // Matches a leading (optionally signed) number followed by an optional unit/suffix,
    // e.g. "21.799999 °C" -> number = "21.799999", unit = "°C".
    private static readonly Regex _numberWithUnit = new Regex(
        @"^\s*(?<number>-?\d+(?:[.,]\d+)?)\s*(?<unit>.*)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Gets or sets the number of decimal places to keep. Defaults to 1, which suits temperatures.
    /// Can be overridden per-binding via the converter parameter (e.g. ConverterParameter=2).
    /// </summary>
    public int Decimals { get; set; } = 1;

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string text = value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        Match match = _numberWithUnit.Match(text);
        if (!match.Success)
        {
            return text;
        }

        // Normalize the decimal separator to '.' so parsing is culture independent.
        string rawNumber = match.Groups["number"].Value.Replace(',', '.');
        if (!double.TryParse(rawNumber, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
        {
            return text;
        }

        int decimals = Math.Max(0, ResolveDecimals(parameter));
        double rounded = Math.Round(number, decimals, MidpointRounding.AwayFromZero);

        // "0.#" style drops insignificant trailing zeros (e.g. 21.0 -> "21", 21.8 -> "21.8").
        string numberFormat = decimals > 0 ? "0." + new string('#', decimals) : "0";
        string formattedNumber = rounded.ToString(numberFormat, CultureInfo.CurrentCulture);

        string unit = match.Groups["unit"].Value.Trim();
        return string.IsNullOrEmpty(unit) ? formattedNumber : $"{formattedNumber} {unit}";
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();

    private int ResolveDecimals(object parameter)
    {
        if (parameter is int intValue)
        {
            return intValue;
        }

        if (parameter is string stringValue &&
            int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
        {
            return parsed;
        }

        return Decimals;
    }
}
