
namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// A message to trigger an item update coming from a server event.
    /// </summary>
    public class ItemStateChangedMessage
    {
        public ItemStateChangedMessage(string itemName, string value, string oldValue)
        {
            ItemName = itemName;
            Value = value;
            OldValue = oldValue;
        }

        /// <summary>
        /// Gets or sets the name of the updated item.
        /// </summary>
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the old item value.
        /// </summary>
        public string OldValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the updated value.
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}