using Windows.UI.Xaml.Controls;
using OpenHAB.Windows.ViewModel;

namespace OpenHAB.Windows.View
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel Vm => DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
