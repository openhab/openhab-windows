// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace OpenHAB.Core.Rest.Models {
    public class EnrichedItemChannelLinkDTO : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The channelUID property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ChannelUID { get; set; }
#nullable restore
#else
        public string ChannelUID { get; set; }
#endif
        /// <summary>The configuration property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public EnrichedItemChannelLinkDTO_configuration? Configuration { get; set; }
#nullable restore
#else
        public EnrichedItemChannelLinkDTO_configuration Configuration { get; set; }
#endif
        /// <summary>The editable property</summary>
        public bool? Editable { get; set; }
        /// <summary>The itemName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ItemName { get; set; }
#nullable restore
#else
        public string ItemName { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="EnrichedItemChannelLinkDTO"/> and sets the default values.
        /// </summary>
        public EnrichedItemChannelLinkDTO() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="EnrichedItemChannelLinkDTO"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static EnrichedItemChannelLinkDTO CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new EnrichedItemChannelLinkDTO();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"channelUID", n => { ChannelUID = n.GetStringValue(); } },
                {"configuration", n => { Configuration = n.GetObjectValue<EnrichedItemChannelLinkDTO_configuration>(EnrichedItemChannelLinkDTO_configuration.CreateFromDiscriminatorValue); } },
                {"editable", n => { Editable = n.GetBoolValue(); } },
                {"itemName", n => { ItemName = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("channelUID", ChannelUID);
            writer.WriteObjectValue<EnrichedItemChannelLinkDTO_configuration>("configuration", Configuration);
            writer.WriteBoolValue("editable", Editable);
            writer.WriteStringValue("itemName", ItemName);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
