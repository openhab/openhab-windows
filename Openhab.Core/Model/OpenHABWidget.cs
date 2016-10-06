using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// A class that represents an OpenHAB widget
    /// </summary>
    public class OpenHABWidget
    {
        /// <summary>
        /// Gets or sets the ID of the OpenHAB widget
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Label of the OpenHAB widget
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the Icon of the OpenHAB widget
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the Type of the OpenHAB widget
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the Url of the OpenHAB widget
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Period of the OpenHAB widget
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Gets or sets the Service of the OpenHAB widget
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets the MinValue of the OpenHAB widget
        /// </summary>
        public float MinValue { get; set; }

        /// <summary>
        /// Gets or sets the MaxValue of the OpenHAB widget
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the Step of the OpenHAB widget
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// Gets or sets the Refresh of the OpenHAB widget
        /// </summary>
        public int Refresh { get; set; }

        /// <summary>
        /// Gets or sets the Height of the OpenHAB widget
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the State of the OpenHAB widget
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the IconColor of the OpenHAB widget
        /// </summary>
        public int IconColor { get; set; }

        /// <summary>
        /// Gets or sets the LabelColor of the OpenHAB widget
        /// </summary>
        public int LabelColor { get; set; }

        /// <summary>
        /// Gets or sets the ValueColor of the OpenHAB widget
        /// </summary>
        public int ValueColor { get; set; }

        /// <summary>
        /// Gets or sets the Encoding of the OpenHAB widget
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// Gets or sets the Item of the OpenHAB widget
        /// </summary>
        public OpenHABItem Item { get; set; }

        /// <summary>
        /// Gets or sets the Parent of the OpenHAB widget
        /// </summary>
        public OpenHABWidget Parent { get; set; }

        /// <summary>
        /// Gets or sets the Children of the OpenHAB widget
        /// </summary>
        [JsonProperty(PropertyName = "widgets")]
        public ICollection<OpenHABWidget> Children { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABWidget"/> class.
        /// </summary>
        public OpenHABWidget()
        {
            Children = new List<OpenHABWidget>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABWidget"/> class.
        /// </summary>
        /// <param name="startNode">The XML from the OpenHAB server that represents this OpenHAB item</param>
        public OpenHABWidget(XElement startNode)
        {
            Children = new List<OpenHABWidget>();
            ParseNode(startNode);
        }

        private void ParseNode(XElement startNode)
        {
            if (!startNode.HasElements)
            {
                return;
            }

            Id = startNode.Element("widgetId")?.Value;
            Type = startNode.Element("type")?.Value;
            Label = startNode.Element("label")?.Value;
            State = startNode.Element("state")?.Value;
            Icon = startNode.Element("icon")?.Value;
            Url = startNode.Element("url")?.Value;

            ParseItem(startNode.Element("item"));
            ParseChildren(startNode);
        }

        private void ParseChildren(XElement startNode)
        {
            foreach (XElement childNode in startNode.Elements("widget"))
            {
                var widget = new OpenHABWidget(childNode) { Parent = this };
                Children.Add(widget);
            }
        }

        private void ParseItem(XElement element)
        {
            if (element == null)
            {
                return;
            }

            Item = new OpenHABItem(element);
        }
    }
}
