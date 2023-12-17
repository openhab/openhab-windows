namespace openHAB.Core.Model
{
    /// <summary>
    /// Enum to differentiate between OpenHAB 1, 2 and 3.
    /// </summary>
    public enum OpenHABVersion
    {
        /// <summary>
        /// OpenHAB V1
        /// </summary>
        One = 1,

        /// <summary>
        /// OpenHAB V2
        /// </summary>
        Two = 2,

        /// <summary>
        /// OpenHAB V3
        /// </summary>
        Three = 3,

        /// <summary>
        /// OpenHAB V4
        /// </summary>
        Four = 4,

        /// <summary>
        /// Used when no connection is available
        /// </summary>
        None,
    }
}
