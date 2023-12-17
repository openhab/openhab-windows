using Microsoft.UI.Xaml.Data;
using openHAB.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts OpenHABUlrState value to corresponding ui glyph.
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class OpenHabUrlStateToGlyph : IValueConverter
    {
        private static Dictionary<ConnectionState, string> _stateToGlyphMapping = new Dictionary<ConnectionState, string>()
        {
            { ConnectionState.Unknown, "\uF142" },
            { ConnectionState.OK, "\uF13E" },
            { ConnectionState.Failed, "\uF13D" },
        };

        /// <summary>Converts the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns a glyph for an url state.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ConnectionState state = ((ConnectionStatusViewModel)value).State;
            _stateToGlyphMapping.TryGetValue(state, out string glyph);

            return glyph;
        }

        /// <summary>Converts the back.</summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns OpenHABUrlState by glyph.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string glyph = value.ToString();

            return _stateToGlyphMapping.FirstOrDefault(x => x.Value.CompareTo(glyph) == 0).Key;
        }
    }
}
