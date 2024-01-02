namespace openHAB.Core.Client.Connection.Models
{
    /// <summary>
    /// Defines the connection type to a local or remote openHAB instance.
    /// </summary>
    public enum HttpClientType
    {
        /// <summary>
        /// Local openHab http connection.
        /// </summary>
        Local,

        /// <summary>
        /// Remote openHab http connection.
        /// </summary>
        Remote,
    }
}
