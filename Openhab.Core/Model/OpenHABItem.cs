using System.Xml.Linq;
using Newtonsoft.Json;

namespace OpenHAB.Core.Model
{
    public class OpenHABItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string GroupType { get; set; }
        public string State { get; set; }
        public string Link { get; set; }

        public OpenHABItem(XElement startNode)
        {
            ParseNode(startNode);
        }

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
            if (startNode.HasElements)
            {
                Name = startNode.Element("name")?.Value;
                Type = startNode.Element("type")?.Value;
                GroupType = startNode.Element("groupType")?.Value;
                State = startNode.Element("state")?.Value;
                Link = startNode.Element("link")?.Value;
            }
        }
    }
}
