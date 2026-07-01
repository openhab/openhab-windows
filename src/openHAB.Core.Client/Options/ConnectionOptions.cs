using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

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
    /// <remarks>
    /// Written with <see cref="JsonSerializer"/>, but loaded via Microsoft.Extensions.Configuration
    /// binding (<c>Configure&lt;ConnectionOptions&gt;</c>). The binder ignores
    /// <see cref="System.Text.Json.Serialization.JsonConverterAttribute"/>, so any read-time
    /// transformation must live in the property accessors, not a converter.
    /// </remarks>
    /// <param name="filePath">The full path of the connection file to write.</param>
    /// <param name="logger">Optional logger used to record a failed save.</param>
    /// <returns><c>true</c> if the save operation was successful; otherwise, <c>false</c>.</returns>
    public bool Save(string filePath, ILogger logger = null)
    {
        try
        {
            string connectionSettings = JsonSerializer.Serialize(this);
            File.WriteAllText(filePath, connectionSettings, Encoding.UTF8);

            return true;
        }
        catch (System.Exception ex)
        {
            logger?.LogError(ex, "Failed to save connection options to '{FilePath}'.", filePath);
            return false;
        }
    }
}
