using System;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
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

        /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
        /// <param name="itemStateManager">The item state manager.</param>
        public NotificationManager(IItemManager itemStateManager)
        {
            Messenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
            _itemManager = itemStateManager;
        }

        private void HandleUpdateItemMessage(ItemStateChangedMessage obj)
        {
            string itemName = obj.ItemName;
            string itemImage = @"C:\Users\chrisho\AppData\Local\Packages\openHABFoundatione.V.openHAB_va1j9qbqnd8h6\LocalCache\icons\heating.png";
            if (_itemManager.TryGetItem(obj.ItemName, out OpenHABItem item))
            {
                itemName = item.Label;
                //itemImage = 
            }

            TriggerToastNotificationForItem(itemName, itemImage, obj.Value, obj.OldValue);
        }

        #region Toast Notification

        public void TriggerToastNotificationForItem(string itemName, string itemImage, string itemValue, string oldItemValue)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            string message = GetMessageFormat(!string.IsNullOrEmpty(oldItemValue));
            message = string.Format(message, itemName, itemValue, oldItemValue);

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

        private string GetMessageFormat(bool oldValueVailable)
        {
            if (oldValueVailable)
            {
                return "{0} changed from {1} to {2}";
            }
            else
            {
                return "{0}: State is {1}";
            }
        }
    }
}
