using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using openHAB.Core.Client.Messages;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A class that represents an OpenHAB item.
    /// </summary>
    public class Item : ObservableObject
    {
        private string _state;
        private string _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        public Item()
        {
            StrongReferenceMessenger.Default.Register<UpdateItemMessage>(this, HandleUpdateItemMessage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="startNode">The XML from the OpenHAB server that represents this OpenHAB item.</param>
        public Item(XElement startNode)
        {
            ParseNode(startNode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="jsonObject">The JSON from the OpenHAB server that represents this OpenHAB item.</param>
        public Item(string jsonObject)
        {
            Item item = JsonSerializer.Deserialize<Item>(jsonObject);
            Name = item.Name;
            Type = item.Type;
            GroupType = item.GroupType;
            State = item.State;
            Link = item.Link;
            CommandDescription = item.CommandDescription;
        }

        /// <summary>
        /// Gets or sets the item category.
        /// </summary>
        /// <value>The category.</value>
        [JsonPropertyName("category")]
        public string Category
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the CommandDescription of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("commandDescription")]
        public CommandDescription CommandDescription
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is editable.
        /// </summary>
        [JsonPropertyName("editable")]
        public bool Editable
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the group names of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("groupNames")]
        public List<string> GroupNames
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the grouptype of the OpenHAB item.
        /// </summary>
        public string GroupType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the item label with the display name.
        /// </summary>
        /// <value>The item label.</value>
        [JsonPropertyName("label")]
        public string Label
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the unit of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("link")]
        public string Link
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the metadata of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Metadata Metadata
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the state of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("state")]
        public string State
        {
            get => _state;
            set
            {
                if (_type != null && Unit == null && _type.Contains(":", StringComparison.OrdinalIgnoreCase) && value != null && value.Contains(" "))
                {
                    int spaceIndex = value.LastIndexOf(' ');
                    Unit = value.Substring(spaceIndex, value.Length - spaceIndex);
                }

                SetProperty(ref _state, value);
            }
        }

        [JsonPropertyName("stateDescription")]
        public StateDescription StateDescription
        {
            get; set;
        }

        [JsonPropertyName("tags")]
        public List<string> Tags
        {
            get; set;
        }

        [JsonPropertyName("transformedState")]
        public string TransformedState
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the type of the OpenHAB item.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get => _type;
            set
            {
                if (value != null && value.Contains(":", StringComparison.OrdinalIgnoreCase) && _state != null)
                {
                    int spaceIndex = _state.LastIndexOf(' ');
                    if (spaceIndex > 0)
                    {
                        Unit = _state.Substring(spaceIndex, _state.Length - spaceIndex);
                    }
                }

                SetProperty(ref _type, value);
            }
        }

        /// <summary>
        /// Gets or sets the link of the OpenHAB item.
        /// </summary>
        public string Unit
        {
            get; set;
        }

        [JsonPropertyName("unitSymbol")]
        public string UnitSymbol
        {
            get; set;
        }

        /// <summary>
        /// Convert state to double value.
        /// </summary>
        /// <returns>
        /// State as double value.
        /// </returns>
        public double GetStateAsDoubleValue()
        {
            string newstate = Regex.Replace(_state, "[^0-9,.]", string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(100));
            double value = 0;
            double.TryParse(newstate, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);

            return value;
        }

        /// <summary>
        /// Send update message to all subscribers.
        /// </summary>
        /// <param name="value">The value.</param>
        public void UpdateValue(object value)
        {
            if (value != null)
            {
                string newValue = value.ToString() + Unit;
                StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(this, newValue));
                _state = newValue;
            }
        }

        private async void HandleUpdateItemMessage(object recipient, UpdateItemMessage message)
        {
            if (message.ItemName != Name)
            {
                return;
            }

            State = message.Value;
        }

        private void ParseNode(XElement startNode)
        {
            if (!startNode.HasElements)
            {
                return;
            }

            Name = startNode.Element("name")?.Value;
            Type = startNode.Element("type")?.Value;
            GroupType = startNode.Element("groupType")?.Value;
            State = startNode.Element("state")?.Value;
            Link = startNode.Element("link")?.Value;
        }
    }
}
