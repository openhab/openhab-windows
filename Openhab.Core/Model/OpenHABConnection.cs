namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Connection configuration for OpenHAB service or cloud instance
    /// </summary>
    public class OpenHABConnection
    {
        /// <summary>
        /// Gets or sets the url to the OpenHAB server.
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the username for the OpenHAB server connection.
        /// </summary>
        public string Username
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password for the OpenHAB connection.
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}
