namespace openHAB.Core.Client.Models
{
    /// <summary>Information about openHAB server.</summary>
    public class ServerInfo
    {
        /// <summary>Gets the openHAB major version.</summary>
        /// <value>The version.</value>
        public OpenHABVersion Version { get; set; }

        /// <summary>Gets the build.</summary>
        /// <value>The build.</value>
        public string Build { get; set; }

        /// <summary>Gets the runtime version.</summary>
        /// <value>The runtime version.</value>
        public string RuntimeVersion { get; set; }
    }
}