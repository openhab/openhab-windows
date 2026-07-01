using System.Collections.Generic;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models;

/// <summary>
/// Manages connection profiles.
/// </summary>
public class ConnectionProfiles
{
    private static readonly Dictionary<int, IConnectionProfile> _profiles = new Dictionary<int, IConnectionProfile>()
        {
            { 1, new DefaultConnectionProfile() },
            { 2, new LocalConnectionProfile() },
            { 3, new RemoteConnectionProfile() },
            { 4, new CloudConnectionProfile() }
        };

    /// <summary>
    /// Gets the connection profile by the specified identifier, or <c>null</c> when no profile matches.
    /// </summary>
    /// <param name="id">The identifier of the connection profile.</param>
    /// <returns>The matching connection profile, or <c>null</c> if the id is unknown.</returns>
    public static IConnectionProfile TryGetProfile(int id)
    {
        return _profiles.TryGetValue(id, out IConnectionProfile profile) ? profile : null;
    }

    /// <summary>
    /// Gets all connection profiles.
    /// </summary>
    /// <returns>A list of all connection profiles.</returns>
    public static List<IConnectionProfile> GetProfiles()
    {
        return new List<IConnectionProfile>(_profiles.Values);
    }
}
