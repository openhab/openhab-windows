using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.Messages
{
    /// <summary>
    /// A message that fires whenever a widget is clicked on a sitemap.
    /// </summary>
    public class WidgetClickedMessage
    {
        /// <summary>
        /// Gets or sets the Widget property.
        /// </summary>
        public WidgetViewModel Widget
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetClickedMessage"/> class.
        /// </summary>
        /// <param name="widget">The widget that was clicked.</param>
        public WidgetClickedMessage(WidgetViewModel widget)
        {
            Widget = widget;
        }
    }
}
