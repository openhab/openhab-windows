using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// Response for REST Root call, which provides all information about openHAB instance.
    /// </summary>
    public class APIInfo
    {
        /// <summary>Gets or sets the version.</summary>
        /// <value>The version.</value>
        [JsonPropertyName("version")]
        public string Version
        {
            get; set;
        }

        /// <summary>Gets or sets the locale.</summary>
        /// <value>The locale.</value>
        [JsonPropertyName("locale")]
        public string Locale
        {
            get; set;
        }

        /// <summary>Gets or sets the measurement system.</summary>
        /// <value>The measurement system.</value>
        [JsonPropertyName("measurementSystem")]
        public string MeasurementSystem
        {
            get; set;
        }

        /// <summary>Gets or sets the runtime information.</summary>
        /// <value>The runtime information.</value>
        [JsonPropertyName("runtimeInfo")]
        public RuntimeInfo RuntimeInfo
        {
            get; set;
        }

        /// <summary>Gets or sets the links.</summary>
        /// <value>The links.</value>
        [JsonPropertyName("links")]
        public List<Link> Links
        {
            get; set;
        }
    }
}
