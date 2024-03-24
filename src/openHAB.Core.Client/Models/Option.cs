using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// Represents an option with a value and label.
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Gets or sets the value of the option.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the label of the option.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label
        {
            get; set;
        }
    }
}
