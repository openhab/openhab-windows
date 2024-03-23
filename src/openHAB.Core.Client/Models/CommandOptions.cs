namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A CommandOptions for commandDescription.
    /// </summary>
    public class CommandOptions
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
        /// Initializes a new instance of the <see cref="CommandOptions"/> class.
        /// </summary>
        /// <param name="command">A command.</param>
        /// <param name="label">A label.</param>
        public CommandOptions(string command, string label)
        {
            Command = command;
            Label = label;
        }
    }
}