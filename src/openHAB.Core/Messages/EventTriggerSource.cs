namespace openHAB.Core.Messages
{
    /// <summary>
    /// Represents the source of the navigation trigger.
    /// </summary>
    public enum EventTriggerSource
    {
        /// <summary>
        /// The navigation trigger is from a breadcrumb.
        /// </summary>
        Breadcrumb,

        /// <summary>
        /// The navigation trigger is from a widget.
        /// </summary>
        Widget,

        /// <summary>
        /// The navigation trigger is from the root.
        /// </summary>
        Root
    }
}
