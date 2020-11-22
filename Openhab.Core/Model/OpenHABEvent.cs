namespace OpenHAB.Core.Model
{
    public class OpenHABEvent
    {
        public string ItemName
        {
            get;
            set;
        }

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

        public string Topic
        {
            get;
            set;
        }

        public string ValueType
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
        public OpenHABEventType EventType
        {
            get;
            internal set;
        }
    }
}