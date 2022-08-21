namespace OpenHAB.Core.Messages
{
    /// <summary>Object for notification with information level.</summary>
    public class FireInfoMessage
    {
        /// <summary>Initializes a new instance of the <see cref="FireInfoMessage" /> class.</summary>
        /// <param name="messageType">Type of the message.</param>
        public FireInfoMessage(MessageType messageType)
        {
            MessageType = messageType;
        }

        /// <summary>Gets the type of the message.</summary>
        /// <value>The type of the message.</value>
        public MessageType MessageType
        {
            get;
            private set;
        }
    }
}
