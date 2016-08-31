using OpenHAB.Core.Model;

namespace OpenHAB.Core.Messages
{
    public class TriggerCommandMessage
    {
        public OpenHABItem Item { get; set; }
        public string Command { get; set; }

        public TriggerCommandMessage(OpenHABItem item, string command)
        {
            Item = item;
            Command = command;
        }
    }
}
