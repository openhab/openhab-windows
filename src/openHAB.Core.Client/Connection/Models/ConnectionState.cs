namespace openHAB.Core.Client.Connection.Models
{
    /// <summary>
    /// Reflects the State for an connection.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// OpenHAB instance is reachable via url
        /// </summary>
        OK,

        /// <summary>
        /// OpenHAB instance can not reach via check url
        /// </summary>
        Failed,

        /// <summary>
        /// State is unknown
        /// </summary>
        Unknown,
    }
}
