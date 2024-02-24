using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using Windows.Media.PlayTo;

namespace openHAB.Core.Messages
{
    /// <summary>
    /// Represents a navigation between two OpenHAB widgets.
    /// </summary>
    public class WigetNavigation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WigetNavigation"/> class.
        /// </summary>
        /// <param name="originWidget">The origin widget.</param>
        /// <param name="targetWidget">The target widget.</param>
        public WigetNavigation(OpenHABWidget originWidget, OpenHABWidget targetWidget, EventTriggerSource trigger)
        {
            OriginWidget = originWidget;
            TargetWidget = targetWidget;
            Trigger = trigger;
        }

        /// <summary>
        /// Gets or sets the trigger source.
        /// </summary>
        public EventTriggerSource Trigger { get; set; }

        /// <summary>
        /// Gets or sets the origin widget.
        /// </summary>
        public OpenHABWidget OriginWidget { get; set; }

        /// <summary>
        /// Gets or sets the target widget.
        /// </summary>
        public OpenHABWidget TargetWidget { get; set; }
    }
}
