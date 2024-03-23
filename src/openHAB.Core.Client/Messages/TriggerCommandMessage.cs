using System;
using openHAB.Core.Client.Models;

namespace openHAB.Core.Client.Messages
{
    /// <summary>
    /// Message that gets fired whenever a widget triggers a command.
    /// </summary>
    public class TriggerCommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerCommandMessage"/> class.
        /// </summary>
        /// <param name="item">The OpenHAB item that triggered the command.</param>
        /// <param name="command">The command that was triggered.</param>
        public TriggerCommandMessage(Item item, string command)
        {
            Id = Guid.NewGuid();
            Item = item;
            Command = command;
        }

        /// <summary>
        /// Gets or sets the Command that was triggered.
        /// </summary>
        public string Command
        {
            get;
            set;
        }

        /// <summary>Gets the unique message identifier.</summary>
        /// <value>The identifier.</value>
        public Guid Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the OpenHAB item that triggered the command.
        /// </summary>
        public Item Item
        {
            get;
            set;
        }
    }
}
