using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using openHAB.Core.Messages;
using openHAB.Core.Model;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB frame.
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
            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(e.ClickedItem as OpenHABWidget));
        }

        internal override void SetState()
        {
        }
    }
}
