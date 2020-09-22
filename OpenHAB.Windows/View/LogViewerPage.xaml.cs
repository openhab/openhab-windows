using OpenHAB.Windows.Services;
using OpenHAB.Windows.ViewModel;
using Windows.UI.Xaml.Controls;

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
            DataContext = (LogsViewModel)DIService.Instance.Services.GetService(typeof(LogsViewModel));

            this.InitializeComponent();
        }
    }
}
