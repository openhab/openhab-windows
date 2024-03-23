using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// A class that represents an OpenHAB widget.
    /// </summary>
    public class Widget : INotifyPropertyChanged, IEquatable<Widget>
    {
        private string _icon;
        private string _label;

        /// <summary>
        /// Initializes a new instance of the <see cref="Widget"/> class.
        /// </summary>
        public Widget()
        {
            Children = new List<Widget>();
        }

        /// <summary>Occurs when a property value changes.</summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the Children of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("widgets")]
        public ICollection<Widget> Children
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Encoding of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("encoding")]
        public string Encoding
        {
            get; set;
        }

        [JsonPropertyName("forceAsItem")]
        public bool ForceAsItem
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Height of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("height")]
        public int Height
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Icon of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon
        {
            get => _icon ?? string.Empty;
            set => _icon = value;
        }

        /// <summary>
        /// Gets or sets the IconColor of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("iconcolor")]
        public string IconColor
        {
            get; set;
        }

        [JsonPropertyName("inputHint")]
        public string InputHint
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Item of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("item")]
        public Item Item
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Label of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label
        {
            get => _label;

            set
            {
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    _label = value;
                    return;
                }

                var parts = value.Split(new[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
                _label = parts[0];

                if (parts.Length > 1)
                {
                    Value = parts[1];
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the LabelColor of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("labelcolor")]
        public string LabelColor
        {
            get; set;
        }

        [JsonPropertyName("labelSource")]
        public string LabelSource
        {
            get; set;
        }

        [JsonPropertyName("legend")]
        public bool Legend
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the linked page when available.
        /// </summary>
        [JsonPropertyName("linkedPage")]
        public Page LinkedPage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Mapping of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("mappings")]
        public ICollection<WidgetMapping> Mappings
        {
            get; set;
        }

        [JsonPropertyName("maxValue")]
        public float MaxValue
        {
            get; set;
        }

        [JsonPropertyName("minValue")]
        public float MinValue
        {
            get; set;
        }

        [JsonPropertyName("name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Parent of the OpenHAB widget.
        /// </summary>
        public Widget Parent
        {
            get; set;
        }

        [JsonPropertyName("pattern")]
        public string Pattern
        {
            get; set;
        }

        [JsonPropertyName("period")]
        public string Period
        {
            get; set;
        }

        [JsonPropertyName("refresh")]
        public float Refresh
        {
            get; set;
        }

        [JsonPropertyName("sendFrequency")]
        public float SendFrequency
        {
            get; set;
        }

        [JsonPropertyName("service")]
        public string Service
        {
            get; set;
        }

        [JsonPropertyName("state")]
        public string State
        {
            get; set;
        }

        [JsonPropertyName("staticIcon")]
        public bool StaticIcon
        {
            get; set;
        }

        [JsonPropertyName("step")]
        public float Step
        {
            get; set;
        }

        [JsonPropertyName("switchSupport")]
        public bool SwitchSupport
        {
            get; set;
        }

        [JsonPropertyName("type")]
        public string Type
        {
            get; set;
        }

        [JsonPropertyName("unit")]
        public string Unit
        {
            get; set;
        }

        [JsonPropertyName("url")]
        public string Url
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Value of the widget.
        /// </summary>
        public string Value
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the ValueColor of the OpenHAB widget.
        /// </summary>
        [JsonPropertyName("valuecolor")]
        public string ValueColor
        {
            get; set;
        }

        [JsonPropertyName("visibility")]
        public bool Visibility
        {
            get; set;
        }

        [JsonPropertyName("widgetId")]
        public string WidgetId
        {
            get; set;
        }

        [JsonPropertyName("yAxisDecimalPattern")]
        public string YAxisDecimalPattern
        {
            get; set;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IEquatable Implementation

        /// <summary>Implements the operator !=.</summary>
        /// <param name="widget1">The widget1.</param>
        /// <param name="widget2">The widget2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Widget widget1, Widget widget2)
        {
            if (ReferenceEquals(widget1, null))
            {
                return false;
            }

            return !widget1.Equals(widget2);
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="widget1">The widget1.</param>
        /// <param name="widget2">The widget2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Widget widget1, Widget widget2)
        {
            if (ReferenceEquals(widget1, null))
            {
                return ReferenceEquals(widget2, null);
            }

            return widget1.Equals(widget2);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            Widget widget = (Widget)obj;

            if (widget == null)
            {
                return false;
            }

            return Equals(widget);
        }

        /// <inheritdoc/>
        public bool Equals(Widget other)
        {
            if (string.IsNullOrEmpty(WidgetId) || ReferenceEquals(other, null))
            {
                return false;
            }

            return WidgetId.CompareTo(other.WidgetId) == 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return WidgetId.GetHashCode();
        }

        #endregion IEquatable Implementation
    }
}