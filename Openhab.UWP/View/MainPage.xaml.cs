using Windows.UI.Xaml.Controls;
using Openhab.ViewModel;

namespace Openhab.UWP.View
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
