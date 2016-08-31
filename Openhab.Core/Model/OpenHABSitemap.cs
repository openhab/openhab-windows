using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenHAB.Core.Model
{
    public class OpenHABSitemap : ObservableBase
    {
        private ICollection<OpenHABWidget> _widgets;
        public string Name { get; set; }
        public string Label { get; set; }
        public string Link { get; set; } 
        public string Icon { get; set; }
        public string HomepageLink { get; set; }
        public bool Leaf { get; set; }

        public ICollection<OpenHABWidget> Widgets
        {
            get { return _widgets; }
            set
            {
                if (Equals(value, _widgets)) return;
                _widgets = value;
                OnPropertyChanged();
            }
        }

        public OpenHABSitemap(XElement startNode)
        {
            ParseNode(startNode);
        }

        private void ParseNode(XElement startNode)
        {
            if (startNode.HasElements)
            {
                Name = startNode.Element("name")?.Value;
                Label = startNode.Element("label")?.Value;
                Link = startNode.Element("link")?.Value;
                Icon = startNode.Element("icon")?.Value;

                var homePage= startNode.Element("homepage");
                HomepageLink = homePage?.Element("link")?.Value;
                Leaf = homePage?.Element("leaf")?.Value == "true";
            }
        }
    }
}
