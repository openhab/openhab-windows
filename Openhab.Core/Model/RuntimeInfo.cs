using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace OpenHAB.Core.Model
{

    public partial class RuntimeInfo
    {
        [JsonProperty("version")]
        public string Version
        {
            get; set;
        }

        [JsonProperty("buildString")]
        public string BuildString
        {
            get; set;
        }
    }
}
