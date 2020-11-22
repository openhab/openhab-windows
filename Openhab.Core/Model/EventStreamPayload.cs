namespace OpenHAB.Core.Model
{
    /// <summary>
    /// The payload coming from the event service.
    /// </summary>
    public class EventStreamPayload
    {
        /// <summary>
        /// Gets or sets the type of event.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the event.
        /// </summary>
        public string Value { get; set; }

        public string OldType
        {
            get;
            set;
        }

        public string OldValue
        {
            get;
            set;
        }
    }
}