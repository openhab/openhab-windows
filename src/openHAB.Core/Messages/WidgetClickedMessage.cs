using OpenHAB.Core.Model;

namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// A message that fires whenever a widget is clicked on a sitemap.
    /// </summary>
    public class WidgetClickedMessage
    {
        /// <summary>
        /// Gets or sets the Widget property.
        /// </summary>
        public OpenHABWidget Widget { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetClickedMessage"/> class.
        /// </summary>
        /// <param name="widget">The widget that was clicked.</param>
        public WidgetClickedMessage(OpenHABWidget widget)
        {
            Widget = widget;
        }
    }
}
