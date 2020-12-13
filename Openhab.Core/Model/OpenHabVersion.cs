namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Enum to differentiate between OpenHAB 1 and 2.
    /// </summary>
    public enum OpenHABVersion
    {
        /// <summary>
        /// OpenHAB V1
        /// </summary>
        One,

        /// <summary>
        /// OpenHAB V2
        /// </summary>
        Two,

        /// <summary>
        /// Used when no connection is available
        /// </summary>
        None,
    }
}
