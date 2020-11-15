using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    public interface IItemStateManager
    {
        bool RegisterOrUpdateItemState(string itemName, object state);

        bool TryGetItemState(string itemName, out ItemState itemState);
    }
}