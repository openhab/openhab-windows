using System.Text.Json.Serialization;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models;


/// <summary>
/// Connection configuration for OpenHAB service or cloud instance.
/// </summary>
public class Connection
{
    /// <summary>Gets or sets the connection profile.</summary>
    /// <value>The profile.</value>
    [JsonIgnore]
    public IConnectionProfile Profile
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the connection profile identifier.
    /// </summary>
    /// <value>The identifier of the connection profile.</value>
    public int ProfileId
    {
        get
        {
            return this.Profile?.Id ?? 0;
        }
        set
        {
            Profile = ConnectionProfiles.TryGetProfile(value);
        }
    }

    /// <summary>Gets or sets the type of the connection.</summary>
    /// <value>The type of the connection.</value>
    public HttpClientType Type
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the URL to the OpenHAB server.
    /// </summary>
    public string Url
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the username for the OpenHAB server connection. Held in plaintext in memory;
    /// persisted encrypted via <see cref="EncryptedUsername"/>.
    /// </summary>
    [JsonIgnore]
    public string Username
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the password for the OpenHAB connection. Held in plaintext in memory;
    /// persisted encrypted via <see cref="EncryptedPassword"/>.
    /// </summary>
    [JsonIgnore]
    public string Password
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the DPAPI-protected username written to and read from the connection file.
    /// </summary>
    public string EncryptedUsername
    {
        get => SecretProtector.Protect(Username);
        set => Username = SecretProtector.Unprotect(value);
    }

    /// <summary>
    /// Gets or sets the DPAPI-protected password written to and read from the connection file.
    /// </summary>
    public string EncryptedPassword
    {
        get => SecretProtector.Protect(Password);
        set => Password = SecretProtector.Unprotect(value);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the application will ignore the SSL certificate.
    /// </summary>
    public bool? WillIgnoreSSLCertificate
    {
        get;
        set;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the application will ignore the SSL hostname.
    /// </summary>
    public bool? WillIgnoreSSLHostname
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets for openHAB MainUI URL.
    /// </summary>
    [JsonIgnore]
    public string MainUIUrl
    {
        get
        {
            if (Profile is LocalConnectionProfile)
            {
                return Url;
            }
            else if (Profile is RemoteConnectionProfile)
            {
                return Url;
            }
            else if (Profile is CloudConnectionProfile)
            {
                return Profile.MainUIUrl;
            }
            else if (Profile is DefaultConnectionProfile)
            {
                return Profile.MainUIUrl;
            }
            else
            {
                return Url;
            }
        }
    }
}
