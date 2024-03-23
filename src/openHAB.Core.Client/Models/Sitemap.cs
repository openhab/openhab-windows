using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{

    /// <summary>
    /// Represents an OpenHAB sitemap.
    /// </summary>
    public class Sitemap
    {
        /// <summary>
        /// Gets or sets the name of the sitemap.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the icon of the sitemap.
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the label of the sitemap.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the link of the sitemap.
        /// </summary>
        [JsonPropertyName("link")]
        public string Link
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the homepage of the sitemap.
        /// </summary>
        [JsonPropertyName("homepage")]
        public Page Homepage
        {
            get; set;
        }
    }
}
