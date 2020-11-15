using System.Collections.Generic;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class ItemStateManager : IItemStateManager
    {
        private Dictionary<string, ItemState> _nameToStateDictionary;

        /// <summary>Initializes a new instance of the <see cref="ItemStateManager" /> class.</summary>
        public ItemStateManager()
        {
            _nameToStateDictionary = new Dictionary<string, ItemState>();
        }

        public bool TryGetItemState(string itemName, out ItemState itemState)
        {
            return _nameToStateDictionary.TryGetValue(itemName, out itemState);
        }

        public bool RegisterOrUpdateItemState(string itemName, object state)
        {
            if (!_nameToStateDictionary.TryGetValue(itemName, out ItemState itemState))
            {
                itemState = new ItemState()
                {
                    Name = itemName,
                    State = state,
                    PreviousState = null,
                };

                return _nameToStateDictionary.TryAdd(itemState.Name, itemState);
            }

            object previousState = itemState.State;

            itemState.State = state;
            itemState.PreviousState = previousState;

            return true;
        }
    }
}
