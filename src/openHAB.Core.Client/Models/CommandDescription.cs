using System.Collections.Generic;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A mapping for an OpenHAB Widget.
    /// </summary>
    public class CommandDescription
    {
        /// <summary>
        /// Gets or sets the CommandOptions.
        /// </summary>
        public ICollection<CommandOptions> CommandOptions { get; set; }

        /// <summary>Initializes a new instance of the <see cref="CommandDescription" /> class.</summary>
        /// <param name="commandOptions">The command options.</param>
        public CommandDescription(ICollection<CommandOptions> commandOptions)
        {
            CommandOptions = commandOptions;
        }
    }
}