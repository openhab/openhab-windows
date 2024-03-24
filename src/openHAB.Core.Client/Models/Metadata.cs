using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// Represents the metadata for an object.
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Gets or sets the value of additional property 1.
        /// </summary>
        [JsonPropertyName("additionalProp1")]
        public object AdditionalProp1
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value of additional property 2.
        /// </summary>
        [JsonPropertyName("additionalProp2")]
        public object AdditionalProp2
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value of additional property 3.
        /// </summary>
        [JsonPropertyName("additionalProp3")]
        public object AdditionalProp3
        {
            get; set;
        }
    }
}
