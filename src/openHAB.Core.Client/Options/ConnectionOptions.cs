using System.IO;
using System.Text;
using System.Text.Json;

namespace openHAB.Core.Client.Options;

/// <summary>
/// Represents the connection options for the OpenHAB application.
/// </summary>
public class ConnectionOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionOptions"/> class.
    /// </summary>
    public ConnectionOptions()
    {
        IsRunningInDemoMode = false;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application is currently running in demo mode.
    /// </summary>
    /// <value><c>true</c> if the application is running in demo mode; otherwise, <c>false</c>.</value>
    public bool? IsRunningInDemoMode
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the configuration to the OpenHAB local instance.
    /// </summary>
    /// <value>The local connection configuration.</value>
    public Connection.Models.Connection LocalConnection
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the configuration to the OpenHAB remote instance.
    /// </summary>
    /// <value>The remote connection configuration.</value>
    public Connection.Models.Connection RemoteConnection
    {
        get;
        set;
    }

    /// <summary>
    /// Saves the current connection options to a file.
    /// </summary>
    /// <returns><c>true</c> if the save operation was successful; otherwise, <c>false</c>.</returns>
    public bool Save(string filePath)
    {
        try
        {
            string connectionSettings = JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, connectionSettings, Encoding.UTF8);

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
