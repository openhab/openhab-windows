namespace openHAB.Core.Notification.Contracts
{
    /// <summary>Class handles toast and tile notifications.</summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Resets the notification count on badge.
        /// </summary>
        void ResetBadgeCount();
    }
}