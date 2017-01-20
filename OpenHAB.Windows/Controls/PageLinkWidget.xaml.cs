using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class PageLinkWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchWidget"/> class.
        /// </summary>
        public PageLinkWidget()
        {
            InitializeComponent();
        }
    }
}