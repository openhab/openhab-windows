

using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{
    /// <summary>Used to serialize the service information from openHAB server.</summary>
    public partial class RuntimeInfo
    {
        /// <summary>Gets or sets the server version.</summary>
        /// <value>The version.</value>
        [JsonPropertyName("version")]
        public string Version
        {
            get; set;
        }

        /// <summary>Gets or sets the build version string.</summary>
        /// <value>The build string.</value>
        [JsonPropertyName("buildString")]
        public string BuildString
        {
            get; set;
        }
    }
}
