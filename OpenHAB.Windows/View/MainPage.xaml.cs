using OpenHAB.Windows.ViewModel;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.View
{
    /// <summary>
    /// Startup page of the application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Gets the datacontext, for use in compiled bindings
        /// </summary>
        public MainViewModel Vm => DataContext as MainViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
