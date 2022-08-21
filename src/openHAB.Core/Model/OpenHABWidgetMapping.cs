namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A mapping for an OpenHAB Widget.
    /// </summary>
    public class OpenHABWidgetMapping
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
        /// Initializes a new instance of the <see cref="OpenHABWidgetMapping"/> class.
        /// </summary>
        /// <param name="command">A command.</param>
        /// <param name="label">A label.</param>
        public OpenHABWidgetMapping(string command, string label)
        {
            Command = command;
            Label = label;
        }
    }
}