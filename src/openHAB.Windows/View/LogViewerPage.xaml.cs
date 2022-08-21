using Microsoft.UI.Xaml.Controls;
using OpenHAB.Windows.Services;
using OpenHAB.Windows.ViewModel;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LogViewerPage : Page
    {
        /// <summary>Initializes a new instance of the <see cref="LogViewerPage" /> class.</summary>
        public LogViewerPage()
        {
            DataContext = DIService.Instance.GetService<LogsViewModel>();

            this.InitializeComponent();
        }
    }
}
