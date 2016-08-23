using System.Collections.Generic;
using System.Xml.Linq;

namespace Openhab.Model
{
    public class OpenHABWidget
    {

        public string Id { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }

        public string IconPath => $"http://jarvis:8080/images/{Icon}.png";

        public string Type { get; set; }
        public string Url { get; set; }
        public string Period { get; set; }
        public string Service { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float Step { get; set; }
        public int Refresh { get; set; }
        public int Height { get; set; }
        public string State { get; set; }
        public int IconColor { get; set; }
        public int LabelColor { get; set; }
        public int ValueColor { get; set; }
        public string Encoding { get; set; }
        public OpenHABItem Item { get; set; }
        public OpenHABWidget Parent { get; set; }
        public ICollection<OpenHABWidget> Children { get; set; }

        public OpenHABWidget(XElement startNode)
        {
            Children = new List<OpenHABWidget>();
            ParseNode(startNode);
        }

        private void ParseNode(XElement startNode)
        {
            if (startNode.HasElements)
            {
                Id = startNode.Element("widgetId")?.Value;
                Type = startNode.Element("type")?.Value;
                Label = startNode.Element("label")?.Value;
                State = startNode.Element("state")?.Value;
                Icon = startNode.Element("icon")?.Value;
                Url = startNode.Element("url")?.Value;

                ParseItem(startNode.Element("item"));
                ParseChildren(startNode);
            }
        }

        private void ParseChildren(XElement startNode)
        {
            foreach (XElement childNode in startNode.Elements("widget"))
            {
                var widget = new OpenHABWidget(childNode) {Parent = this};
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
