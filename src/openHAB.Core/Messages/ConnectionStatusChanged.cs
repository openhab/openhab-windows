using openHAB.Core.Common;
using openHAB.Core.Model;

namespace openHAB.Core.Messages
{
    /// <summary>
    /// Event for connection state updates.
    /// </summary>
    public class ConnectionStatusChanged
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectionStatusChanged" /> class.</summary>
        /// <param name="state">The connection state.</param>
        public ConnectionStatusChanged(ConnectionState state)
        {
            State = state;
        }

        /// <summary>Gets or sets the state.</summary>
        /// <value>The state.</value>
        public ConnectionState State
        {
            get; set;
        }
    }
}
