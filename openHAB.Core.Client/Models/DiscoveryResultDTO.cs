// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace openHAB.Core.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class DiscoveryResultDTO
    {
        /// <summary>
        /// Initializes a new instance of the DiscoveryResultDTO class.
        /// </summary>
        public DiscoveryResultDTO()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the DiscoveryResultDTO class.
        /// </summary>
        /// <param name="flag">Possible values include: 'NEW',
        /// 'IGNORED'</param>
        public DiscoveryResultDTO(string bridgeUID = default(string), string flag = default(string), string label = default(string), IDictionary<string, object> properties = default(IDictionary<string, object>), string representationProperty = default(string), string thingUID = default(string), string thingTypeUID = default(string))
        {
            BridgeUID = bridgeUID;
            Flag = flag;
            Label = label;
            Properties = properties;
            RepresentationProperty = representationProperty;
            ThingUID = thingUID;
            ThingTypeUID = thingTypeUID;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "bridgeUID")]
        public string BridgeUID { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'NEW', 'IGNORED'
        /// </summary>
        [JsonProperty(PropertyName = "flag")]
        public string Flag { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "representationProperty")]
        public string RepresentationProperty { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "thingUID")]
        public string ThingUID { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "thingTypeUID")]
        public string ThingTypeUID { get; set; }

    }
}
