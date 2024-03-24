using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{

    /// <summary>
    /// Represents the description of a state.
    /// </summary>
    public class StateDescription
    {
        /// <summary>
        /// Gets or sets the minimum value of the state.
        /// </summary>
        [JsonPropertyName("minimum")]
        public float Minimum
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maximum value of the state.
        /// </summary>
        [JsonPropertyName("maximum")]
        public float Maximum
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the step value of the state.
        /// </summary>
        [JsonPropertyName("step")]
        public float Step
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the pattern of the state.
        /// </summary>
        [JsonPropertyName("pattern")]
        public string Pattern
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state is read-only.
        /// </summary>
        [JsonPropertyName("readOnly")]
        public bool ReadOnly
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the list of options for the state.
        /// </summary>
        [JsonPropertyName("options")]
        public List<Option> Options
        {
            get; set;
        }
    }
}
