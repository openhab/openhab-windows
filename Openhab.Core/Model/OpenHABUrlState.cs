namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Reflects the State for an url check.
    /// </summary>
    public enum OpenHABUrlState
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
