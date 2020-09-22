namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// A message to trigger an item update coming from a server event.
    /// </summary>
    public class UpdateItemMessage
    {
        /// <summary>
        /// Gets or sets the name of the updated item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the updated value.
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        public UpdateItemMessage(string itemName, string value)
        {
            ItemName = itemName;
            Value = value;
        }
    }
}