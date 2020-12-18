using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace OpenHAB.Core.Model
{

    public partial class Link
    {
        [JsonProperty("type")]
        public string Type
        {
            get; set;
        }

        [JsonProperty("url")]
        public Uri Url
        {
            get; set;
        }
    }
}
