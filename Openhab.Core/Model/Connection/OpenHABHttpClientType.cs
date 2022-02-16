namespace OpenHAB.Core.Model.Connection
{
    /// <summary>
    /// Defines the connection type to a local or remote openHAB instance.
    /// </summary>
    public enum OpenHabHttpClientType
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
