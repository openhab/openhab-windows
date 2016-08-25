using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Openhab.Model;

namespace Openhab.UWP.Controls
{
    public class WidgetBase : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register(
            nameof(Widget), typeof(OpenHABWidget), typeof(WidgetBase), new PropertyMetadata(default(OpenHABWidget)));

        public OpenHABWidget Widget
        {
            get { return (OpenHABWidget) GetValue(WidgetProperty); }
            set { SetValue(WidgetProperty, value); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
