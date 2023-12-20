using System;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using openHAB.Core.Common;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Notification
{
    /// <inheritdoc/>
    public class NotificationManager : INotificationManager
    {
        private IItemManager _itemManager;
        private string _iconFormat;
        private ISettingsService _settingsService;

        /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
        /// <param name="itemStateManager">The item state manager.</param>
        /// <param name="settingsService">Setting service.</param>
        /// <param name="settings">Application Settings.</param>
        public NotificationManager(IItemManager itemStateManager, ISettingsService settingsService, Settings settings)
        {
            StrongReferenceMessenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
            _itemManager = itemStateManager;
            _iconFormat = settings.UseSVGIcons ? "svg" : "png";
            _settingsService = settingsService;
        }

        private void HandleUpdateItemMessage(object receipts, ItemStateChangedMessage obj)
        {
            Settings settings = _settingsService.Load();

            string itemName = obj.ItemName;
            string itemImage = string.Empty;
            if (_itemManager.TryGetItem(obj.ItemName, out OpenHABItem item))
            {
                itemName = item.Label;
                string state = item?.State ?? "ON";
                itemImage = $"{OpenHABHttpClient.BaseUrl}icon/{item.Category}?state={state}&format={_iconFormat}";
            }

            if (settings.NotificationsEnable.HasValue && !settings.NotificationsEnable.Value)
            {
                return;
            }

            TriggerToastNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);
        }

        #region Toast Notification

        private void TriggerToastNotificationForItem(string itemName, string itemImage, string value, string oldValue)
        {
            string message = GetMessage(itemName, value, oldValue, "NotificationToast", "NotificationToastSimple");
            ToastContentBuilder contentBuilder = CreateToastMessage(message, itemImage);
            contentBuilder.Show();
        }

        private ToastContentBuilder CreateToastMessage(string message, string image)
        {
            ToastContentBuilder toastContentBuilder = new ToastContentBuilder()
                .AddArgument("action", "show")
                .AddArgument("conversationId", 9813)
                .AddText("openHAB for Windows")
                .AddInlineImage(new Uri(image))
                .AddText(message);

            return toastContentBuilder;
        }

        #endregion

        private string GetMessage(
            string itemName,
            string itemValue,
            string oldItemValue,
            string valueChangedMessageRessource,
            string stateChangedMessageRessource)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(oldItemValue))
            {
                message = AppResources.Values.GetString(valueChangedMessageRessource);
            }
            else
            {
                message = AppResources.Values.GetString(stateChangedMessageRessource);
            }

            message = string.Format(CultureInfo.InvariantCulture, message, itemName, oldItemValue, itemValue);

            return message;
        }
    }
}
