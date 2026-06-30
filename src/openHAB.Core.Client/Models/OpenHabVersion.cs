namespace openHAB.Core.Client.Models;

/// <summary>
/// Enum to differentiate between the supported openHAB major versions.
/// </summary>
public enum OpenHABVersion
{
    /// <summary>
    /// Used when no connection is available.
    /// </summary>
    None = 0,

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
    /// OpenHAB V5
    /// </summary>
    Five = 5,
}
