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
            OpenHABWidgetMapping itemState = Widget?.Mappings.FirstOrDefault(_ => _.Command == Widget.Item.State);
            SelectionComboBox.SelectedItem = itemState;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenHABWidgetMapping mapping = e.AddedItems.FirstOrDefault() as OpenHABWidgetMapping;

            if (mapping == null)
            {
                return;
            }

            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, mapping.Command));
        }
    }
}