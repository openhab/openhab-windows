using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SectionSwitchWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionSwitchWidget"/> class.
        /// </summary>
        public SectionSwitchWidget()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, button?.Tag.ToString()));
        }
    }
}