using System;
using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{
    /// <summary>Use to serialize api information from openHAB server.</summary>
    public partial class Link
    {
        /// <summary>Gets or sets the type.</summary>
        /// <value>The type.</value>
        [JsonPropertyName("type")]
        public string Type
        {
            get; set;
        }

        /// <summary>Gets or sets the URL.</summary>
        /// <value>The URL.</value>
        [JsonPropertyName("url")]
        public Uri Url
        {
            get; set;
        }
    }
}
