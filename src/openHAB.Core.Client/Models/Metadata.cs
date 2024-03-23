using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace openHAB.Core.Client.Models
{
    public partial class Metadata
    {
        [JsonPropertyName("additionalProp1")]
        public object AdditionalProp1
        {
            get; set;
        }

        [JsonPropertyName("additionalProp2")]
        public object AdditionalProp2
        {
            get; set;
        }

        [JsonPropertyName("additionalProp3")]
        public object AdditionalProp3
        {
            get; set;
        }
    }
}
