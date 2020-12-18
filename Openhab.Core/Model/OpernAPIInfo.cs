using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Response for REST Root call, which provides all information about openHAB instance.
    /// </summary>
    public partial class OpenHabAPIInfo
    {
        [JsonProperty("version")]
        public int Version
        {
            get; set;
        }

        [JsonProperty("locale")]
        public string Locale
        {
            get; set;
        }

        [JsonProperty("measurementSystem")]
        public string MeasurementSystem
        {
            get; set;
        }

        [JsonProperty("runtimeInfo")]
        public RuntimeInfo RuntimeInfo
        {
            get; set;
        }

        [JsonProperty("links")]
        public List<Link> Links
        {
            get; set;
        }
    }
}
