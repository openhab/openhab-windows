using System;
using System.Globalization;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace OpenHAB.Core.Services
{
    /// <inheritdoc/>
    public class NotificationManager : INotificationManager
    {
        private IItemManager _itemManager;
        private string _iconFormat;

        /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
        /// <param name="itemStateManager">The item state manager.</param>
        public NotificationManager(IItemManager itemStateManager, ISettingsService settingsService)
        {
            Messenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
            _itemManager = itemStateManager;

            Settings settings = settingsService.Load();
            _iconFormat = settings.UseSVGIcons ? "svg" : "png";
        }

        private void HandleUpdateItemMessage(ItemStateChangedMessage obj)
        {
            string itemName = obj.ItemName;
            string itemImage = string.Empty;
            if (_itemManager.TryGetItem(obj.ItemName, out OpenHABItem item))
            {
                itemName = item.Label;

                string state = item?.State ?? "ON";

                itemImage = $"{OpenHABHttpClient.BaseUrl}icon/{item.Category}?state={state}&format={_iconFormat}";
            }

            TriggerToastNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);
        }

        #region Toast Notification

        public void TriggerToastNotificationForItem(string itemName, string itemImage, string itemValue, string oldItemValue)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            string message = GetItemChangeMessage(itemName, itemValue, oldItemValue);
            var xmdock = CreateToastMessage(message, itemImage);
            var toast = new ToastNotification(xmdock);

            notifier.Show(toast);
        }

        private static XmlDocument CreateToastMessage(string message, string image)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "openHAB for Windows",
                                HintMaxLines = 1,
                            },
                            new AdaptiveText()
                            {
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Show Item", "action=show")
                        {
                            ActivationType = ToastActivationType.Foreground,
                        }
                    }
                }
            };

            return toastContent.GetXml();
        }

        #endregion

        private string GetItemChangeMessage(string itemName, string itemValue, string oldItemValue)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(oldItemValue))
            {
                message = "{0} changed from {1} to {2}";
            }
            else
            {
                message = "{0}: State is {1}";
            }

            message = string.Format(CultureInfo.InvariantCulture, message, itemName, oldItemValue, itemValue);

            return message;
        }
    }
}
