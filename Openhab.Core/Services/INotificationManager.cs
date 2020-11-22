using OpenHAB.Core.Model;

namespace OpenHAB.Core.Services
{
    public interface INotificationManager
    {
        /// <summary>Triggers the toast notification for a item event.</summary>
        /// <param name="itemName">Item internal name.</param>
        /// <param name="itemValue">Current item value.</param>
        /// <param name="oldItemValue">Old item value.</param>
        void TriggerToastNotificationForItem(string itemName, string itemValue, string oldItemValue);
    }
}