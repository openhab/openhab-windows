namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A mapping for an OpenHAB Widget.
    /// </summary>
    public class WidgetMapping
    {
        /// <summary>
        /// Gets or sets the Command of the mapping.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the Label of the mapping.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetMapping"/> class.
        /// </summary>
        /// <param name="command">A command.</param>
        /// <param name="label">A label.</param>
        public WidgetMapping(string command, string label)
        {
            Command = command;
            Label = label;
        }
    }
}