using System.Collections.Generic;
using System.Xml.Linq;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A class that represents an OpenHAB sitemap.
    /// </summary>
    public class OpenHABSitemap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABSitemap"/> class.
        /// </summary>
        public OpenHABSitemap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABSitemap"/> class.
        /// </summary>
        /// <param name="startNode">The XML from the OpenHAB server that represents this OpenHAB item.</param>
        public OpenHABSitemap(XElement startNode)
        {
            ParseNode(startNode);
        }

        /// <summary>
        /// Gets or sets the name of the OpenHAB sitemap.
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the label of the OpenHAB sitemap.
        /// </summary>
        public string Label
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the link of the OpenHAB sitemap.
        /// </summary>
        public string Link
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the icon of the OpenHAB sitemap.
        /// </summary>
        public string Icon
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the url of the OpenHAB sitemap.
        /// </summary>
        public string HomepageLink
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sitemap is a leaf.
        /// </summary>
        public bool Leaf
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the title of the sitemap.
        /// </summary>
        public string Title
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a collection of widgets of the OpenHAB sitemap.
        /// </summary>
        public ICollection<OpenHABWidget> Widgets
        {
            get;
            set;
        }

        private void ParseNode(XElement startNode)
        {
            if (startNode.HasElements)
            {
                Name = startNode.Element("name")?.Value;
                Label = startNode.Element("label")?.Value;
                Link = startNode.Element("link")?.Value;
                Icon = startNode.Element("icon")?.Value;

                var homePage = startNode.Element("homepage");
                HomepageLink = homePage?.Element("link")?.Value;
                Leaf = homePage?.Element("leaf")?.Value == "true";
            }
        }
    }
}
