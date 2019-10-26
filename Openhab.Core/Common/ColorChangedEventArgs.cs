using System;
using Windows.UI;

namespace OpenHAB.Core.Common
{
    /// <summary>
    /// Event arguments for a color changed event.
    /// </summary>
    public class ColorChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the newly selected color.
        /// </summary>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="selectedColor">The color that was just selected.</param>
        public ColorChangedEventArgs(Color selectedColor)
        {
            SelectedColor = selectedColor;
        }
    }
}
