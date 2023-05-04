using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace openHAB.Windows.Controls
{
    /// <summary>Dialog control for connection configruation.</summary>
    /// <seealso cref="ContentDialog" />
    /// <seealso cref="IComponentConnector" />
    public sealed partial class ConnectionDialog : ContentDialog
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectionDialog"/> class.</summary>
        public ConnectionDialog()
        {
            this.InitializeComponent();
        }
    }
}
