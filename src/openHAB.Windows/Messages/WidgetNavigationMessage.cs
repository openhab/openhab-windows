using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.Messages
{
    /// <summary>
    /// Represents a navigation between two OpenHAB widgets.
    /// </summary>
    public class WidgetNavigationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetNavigationMessage"/> class.
        /// </summary>
        /// <param name="originWidget">The origin widget.</param>
        /// <param name="targetWidget">The target widget.</param>
        public WidgetNavigationMessage(WidgetViewModel originWidget, WidgetViewModel targetWidget, EventTriggerSource trigger)
        {
            OriginWidget = originWidget;
            TargetWidget = targetWidget;
            Trigger = trigger;
        }

        /// <summary>
        /// Gets or sets the trigger source.
        /// </summary>
        public EventTriggerSource Trigger
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the origin widget.
        /// </summary>
        public WidgetViewModel OriginWidget
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the target widget.
        /// </summary>
        public WidgetViewModel TargetWidget
        {
            get; set;
        }
    }
}
