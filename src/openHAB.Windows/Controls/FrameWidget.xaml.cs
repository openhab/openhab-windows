using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using openHAB.Windows.Messages;
using openHAB.Windows.ViewModel;

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
            WidgetViewModel viewModel = e.ClickedItem as WidgetViewModel;
            if (viewModel == null)
            {
                return;
            }

            StrongReferenceMessenger.Default.Send(new WidgetClickedMessage(viewModel));
        }

        internal override void SetState()
        {
        }
    }
}
