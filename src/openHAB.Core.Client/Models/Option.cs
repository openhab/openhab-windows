using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{
    public class Option
    {
        [JsonPropertyName("value")]
        public string Value
        {
            get; set;
        }

        [JsonPropertyName("label")]
        public string Label
        {
            get; set;
        }
    }
}
