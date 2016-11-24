using System.Xml.Linq;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A class that represents an OpenHAB item
    /// </summary>
    public class OpenHABItem
    {
        /// <summary>
        /// Gets or sets the name of the OpenHAB item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the OpenHAB item
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the grouptype of the OpenHAB item
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// Gets or sets the state of the OpenHAB item
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the link of the OpenHAB item
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABItem"/> class.
        /// </summary>
        public OpenHABItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABItem"/> class.
        /// </summary>
        /// <param name="startNode">The XML from the OpenHAB server that represents this OpenHAB item</param>
        public OpenHABItem(XElement startNode)
        {
            ParseNode(startNode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABItem"/> class.
        /// </summary>
        /// <param name="jsonObject">The JSON from the OpenHAB server that represents this OpenHAB item</param>
        public OpenHABItem(string jsonObject)
        {
            var item = JsonConvert.DeserializeObject<OpenHABItem>(jsonObject);
            Name = item.Name;
            Type = item.Type;
            GroupType = item.GroupType;
            State = item.State;
            Link = item.Link;
        }

        private void ParseNode(XElement startNode)
        {
            if (!startNode.HasElements)
            {
                return;
            }

            Name = startNode.Element("name")?.Value;
            Type = startNode.Element("type")?.Value;
            GroupType = startNode.Element("groupType")?.Value;
            State = startNode.Element("state")?.Value;
            Link = startNode.Element("link")?.Value;
        }
    }
}
