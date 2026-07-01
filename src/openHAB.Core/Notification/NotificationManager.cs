using System;
using System.Globalization;
using System.Web;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using openHAB.Common;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Notification;

/// <inheritdoc/>
public class NotificationManager : INotificationManager
{
    private readonly IItemManager _itemManager;
    private readonly string _iconFormat;
    private readonly IIconCaching _iconCaching;
    private readonly IOptions<SettingOptions> _settingsOption;

    /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
    /// <param name="itemStateManager">The item state manager.</param>
    /// <param name="iconCaching">Service for Icon caching.</param>
    /// <param name="settings">Application Settings.</param>
    public NotificationManager(IItemManager itemStateManager, IIconCaching iconCaching, IOptions<SettingOptions> settingsOption)
    {
        StrongReferenceMessenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
        _itemManager = itemStateManager;
        _iconCaching = iconCaching;

        _settingsOption = settingsOption;
        SettingOptions settings = _settingsOption.Value;
        _iconFormat = settings.UseSVGIcons ? "svg" : "svg";
    }

    private async void HandleUpdateItemMessage(object receipts, ItemStateChangedMessage obj)
    {
        SettingOptions settings = _settingsOption.Value;
        if (!settings.NotificationsEnable)
        {
            return;
        }

        string itemName = obj.ItemName;
        string itemImage = string.Empty;
        string itemPath = string.Empty;
        if (_itemManager.TryGetItem(obj.ItemName, out Item item))
        {
            itemName = item?.Label ?? "NA";
            string state = item?.State ?? "ON";
            state = HttpUtility.UrlEncode(state);

            string icon = item?.Category?.ToLower();
            itemImage = await _iconCaching.ResolveIconPath(icon, state, _iconFormat);
        }

        TriggerToastNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);
    }

    #region Toast Notification

    private void TriggerToastNotificationForItem(string itemName, string itemImage, string value, string oldValue)
    {
        string message = GetMessage(itemName, value, oldValue, "NotificationToast", "NotificationToastSimple");
        AppNotificationBuilder contentBuilder = CreateAppNotification(itemName, message, itemImage);
        AppNotification notification = contentBuilder.BuildNotification();

        AppNotificationManager.Default.Show(notification);
    }

    private AppNotificationBuilder CreateAppNotification(string itemName, string message, string image)
    {
        AppNotificationBuilder notificationBuilder = new AppNotificationBuilder()
            .AddArgument("action", "show")
            .AddArgument("item", itemName)
            .AddText("openHAB for Windows")
            .AddText(message);

        if (string.IsNullOrEmpty(image))
        {
            image = "ms-appx:///Assets/openhab-logo-square.png";
        }

        notificationBuilder = notificationBuilder.SetAppLogoOverride(new Uri(image), AppNotificationImageCrop.Circle);

        return notificationBuilder;
    }

    #endregion Toast Notification

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
