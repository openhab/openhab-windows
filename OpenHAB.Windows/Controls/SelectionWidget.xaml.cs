using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SelectionWidget : WidgetBase
    {
        private System.Collections.Generic.List<SelectionMapping> selectionMappings;
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionWidget"/> class.
        /// </summary>
        public SelectionWidget()
        {
            InitializeComponent();
            Loaded += SelectionWidget_Loaded;
        }


        private void SelectionWidget_Loaded(object sender, global::Windows.UI.Xaml.RoutedEventArgs e)
        {
            selectionMappings = new System.Collections.Generic.List<SelectionMapping>();
            if (Widget?.Item.commandDescription?.CommandOptions?.Count > 0)
            {
                foreach (OpenHABCommandOptions option in Widget.Item.commandDescription.CommandOptions)
                {
                    SelectionMapping mapping = new SelectionMapping(option.Command, option.Label);
                    selectionMappings.Add(mapping);
                }
            }

            if (Widget?.Mappings.Count > 0)
            {
                foreach (OpenHABWidgetMapping option in Widget.Mappings)
                {
                    SelectionMapping mapping = new SelectionMapping(option.Command, option.Label);
                    selectionMappings.Add(mapping);
                }
            }

            SelectionComboBox.ItemsSource = selectionMappings;
            SetState();
        }

        internal override void SetState()
        {
            //OpenHABWidgetMapping itemState = Widget?.Mappings.FirstOrDefault(_ => _.Command == Widget.Item.State);
            SelectionMapping itemState = selectionMappings.FirstOrDefault(_ => _.Command == Widget.Item.State);
            SelectionComboBox.SelectedItem = itemState;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenHABWidgetMapping mapping = (OpenHABWidgetMapping)e.AddedItems.FirstOrDefault();

            if (mapping == null)
            {
                return;
            }

            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, mapping.Command));
        }

        class SelectionMapping
        {
            /// <summary>
            /// Gets or sets the Command of the mapping
            /// </summary>
            public string Command { get; set; }

            /// <summary>
            /// Gets or sets the Label of the mapping
            /// </summary>
            public string Label { get; set; }

            public SelectionMapping(string command, string label)
            {
                Command = command;
                Label = label;
            }
        }
    }
}