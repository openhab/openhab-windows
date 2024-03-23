using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{
    public class StateDescription
    {
        [JsonPropertyName("minimum")]
        public float Minimum
        {
            get; set;
        }

        [JsonPropertyName("maximum")]
        public float Maximum
        {
            get; set;
        }

        [JsonPropertyName("step")]
        public float Step
        {
            get; set;
        }

        [JsonPropertyName("pattern")]
        public string Pattern
        {
            get; set;
        }

        [JsonPropertyName("readOnly")]
        public bool ReadOnly
        {
            get; set;
        }

        [JsonPropertyName("options")]
        public List<Option> Options
        {
            get; set;
        }
    }
}
