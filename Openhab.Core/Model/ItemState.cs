namespace OpenHAB.Core.Model
{
    public class ItemState
    {
        public string Name
        {
            get;
            set;
        }

        public object State
        {
            get;
            set;
        }

        public object PreviousState
        {
            get;
            set;
        }
    }
}