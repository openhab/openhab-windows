using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models;

/// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
/// <seealso cref="IConnectionProfile" />
public class CloudConnectionProfile : IConnectionProfile
{
    /// <inheritdoc/>
    public bool AllowHostUrlConfiguration
    {
        get => false;
    }

    /// <inheritdoc/>
    public bool AllowIgnoreSSLCertificate
    {
        get => false;
    }

    /// <inheritdoc/>
    public bool AllowIgnoreSSLHostname
    {
        get => false;
    }

    /// <inheritdoc/>
    public int Id
    {
        get => 4;
    }

    /// <inheritdoc/>
    public string Name
    {
        get => AppResources.Values.GetString("RemoteDefaultConnection");
    }

    /// <inheritdoc/>
    public HttpClientType Type
    {
        get => HttpClientType.Remote;
    }

    public string MainUIUrl
    {
        get => "https://home.myopenhab.org/";
    }

    public string Url => "https://myopenhab.org/";

    /// <inheritdoc/>
    public Connection CreateConnection()
    {
        return new Connection()
        {
            Profile = this,
            Type = HttpClientType.Remote,
            Url = Url
        };
    }
}
