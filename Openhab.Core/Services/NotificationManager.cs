using System;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Notifications;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace OpenHAB.Core.Services
{
    public class NotificationManager : INotificationManager
    {
        private IItemStateManager _itemStateManager;

        /// <summary>Initializes a new instance of the <see cref="NotificationManager" /> class.</summary>
        /// <param name="itemStateManager">The item state manager.</param>
        public NotificationManager(IItemStateManager itemStateManager)
        {
            Messenger.Default.Register<ItemStateChangedMessage>(this, HandleUpdateItemMessage);
            _itemStateManager = itemStateManager;
        }

        private void HandleUpdateItemMessage(ItemStateChangedMessage obj)
        {
            _itemStateManager.RegisterOrUpdateItemState(obj.ItemName, obj.Value);
            TriggerToastNotificationForItem(obj.ItemName, obj.Value, obj.OldValue);
        }

        public void TriggerToastNotificationForItem(string itemName, string itemValue, string oldItemValue)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            string message = GetMessageFormat(!string.IsNullOrEmpty(oldItemValue));
            message = string.Format(message, itemName, itemValue, oldItemValue);

            var xmdock = CreateToastMessage(message);
            var toast = new ToastNotification(xmdock);

            notifier.Show(toast);
        }

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

        private static XmlDocument CreateToastMessage(string message)
        {
            var xDoc = new XDocument(
               new XElement("toast",
                   new XElement("visual",
                       new XElement("binding", new XAttribute("template", "ToastGeneric"),
                                   new XElement("text", "openHAB for Windows"),
                                   new XElement("text", message))),
                   new XElement("actions",
                         new XElement("action", new XAttribute("activationType", "background"),
                                                new XAttribute("content", "Show Item"), new XAttribute("arguments", "show")),
                         new XElement("action", new XAttribute("activationType", "background"),
                                                new XAttribute("content", "Discard notification"), new XAttribute("arguments", "discard")))));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());

            return xmlDoc;
        }
    }
}
