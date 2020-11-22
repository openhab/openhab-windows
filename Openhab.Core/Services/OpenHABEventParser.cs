using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    public class OpenHABEventParser
    {
        public static OpenHABEvent Parse(string message)
        {
            var data = JsonConvert.DeserializeObject<EventStreamData>(message.Remove(0, 6));

            var payload = JsonConvert.DeserializeObject<EventStreamPayload>(data.Payload);
            string itemName = data.Topic.Replace("smarthome/items/", string.Empty).Replace("/statechanged", string.Empty).Replace("/state", string.Empty);

            if (!Enum.TryParse(typeof(OpenHABEventType), data.Type, out object type))
            {
                type = OpenHABEventType.Unknown;
            }

            OpenHABEvent openHABevent = new OpenHABEvent()
            {
                ItemName = itemName,
                ValueType = payload.Type,
                Value = payload.Value,
                Topic = data.Topic,
                OldType = payload.OldType,
                OldValue = payload.OldValue,
                EventType = (OpenHABEventType)type,
            };

            return openHABevent;
        }
    }
}
