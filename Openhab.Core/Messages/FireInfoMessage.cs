using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHAB.Core.Messages
{
    public class FireInfoMessage
    {
        public FireInfoMessage(MessageType messageType)
        {
            MessageType = messageType;
        }

        public MessageType MessageType
        {
            get;
            private set;
        }
    }
}
