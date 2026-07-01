using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models;

/// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
/// <seealso cref="IConnectionProfile" />
public class DemoConnectionProfile : IConnectionProfile
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
        get => 5;
    }

    /// <inheritdoc/>
    public string Name
    {
        get => AppResources.Values.GetString("DemoConnection");
    }

    /// <inheritdoc/>
    public HttpClientType Type
    {
        get => HttpClientType.Remote;
    }

    /// <inheritdoc/>
    public string MainUIUrl => Url;

    /// <inheritdoc/>
    public string Url => Constants.API.DemoModeUrl;

    /// <inheritdoc/>
    public Connection CreateConnection()
    {
        return new Connection()
        {
            Profile = this,
            Type = HttpClientType.Remote,
            Url = Url,
            Username = Constants.API.DemoModeUser,
            Password = Constants.API.DemoModeUserPwd
        };
    }
}
