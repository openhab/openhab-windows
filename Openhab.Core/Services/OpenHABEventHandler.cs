using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    public class OpenHABEventHandler
    {
        public static OpenHABEvent ParseEventMessage(string message)
        {
            var data = JsonConvert.DeserializeObject<EventStreamData>(message.Remove(0, 6));
            if (!data.Topic.EndsWith("state", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var payload = JsonConvert.DeserializeObject<EventStreamPayload>(data.Payload);

            string itemName = data.Topic.Replace("smarthome/items/", string.Empty).Replace("/state", string.Empty);

            OpenHABEvent openHABevent = new OpenHABEvent()
            {
                ItemName = itemName,
                Type = payload.Type,
                Value = payload.Value,
                Topic = data.Topic,
            };

            return openHABevent;
        }
    }
}
