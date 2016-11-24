using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB frame
    /// </summary>
    public sealed partial class FrameWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameWidget"/> class.
        /// </summary>
        public FrameWidget()
        {
            InitializeComponent();
        }

        private void OnItemClick(object sender, ItemClickEventArgs e)
        {
            Messenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }
    }
}
