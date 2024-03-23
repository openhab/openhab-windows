using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// Represents a page in the openHAB application.
    /// </summary>
    public class Page
    {
        /// <summary>
        /// Gets or sets the ID of the page.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the title of the page.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the icon of the page.
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the link of the page.
        /// </summary>
        [JsonPropertyName("link")]
        public string Link
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the parent page ID.
        /// </summary>
        [JsonPropertyName("parent")]
        public string Parent
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page is a leaf page.
        /// </summary>
        [JsonPropertyName("leaf")]
        public bool Leaf
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page has a timeout.
        /// </summary>
        [JsonPropertyName("timeout")]
        public bool Timeout
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the list of widgets on the page.
        /// </summary>
        [JsonPropertyName("widgets")]
        public List<Widget> Widgets
        {
            get; set;
        }
    }
}
