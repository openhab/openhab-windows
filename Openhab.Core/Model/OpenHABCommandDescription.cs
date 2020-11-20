using System.Collections.Generic;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A mapping for an OpenHAB Widget
    /// </summary>
    public class OpenHABCommandDescription
    {
        /// <summary>
        /// Gets or sets the CommandOptions
        /// </summary>
        public ICollection<OpenHABCommandOptions> CommandOptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABWidgetMapping"/> class.
        /// </summary>
        /// <param name="command">A command</param>
        /// <param name="label">A label</param>
        public OpenHABCommandDescription(ICollection<OpenHABCommandOptions> commandoptions)
        {
            CommandOptions = commandoptions;
        }
    }
}