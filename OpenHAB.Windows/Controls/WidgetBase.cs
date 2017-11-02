using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// A base class for all OpenHAB widget controls
    /// </summary>
    public abstract class WidgetBase : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// A bindable property to bind the OpenHAB widget to the control
        /// </summary>
        public static readonly DependencyProperty WidgetProperty = DependencyProperty.Register(
            nameof(Widget), typeof(OpenHABWidget), typeof(WidgetBase), new PropertyMetadata(default(OpenHABWidget)));

        /// <summary>
        /// Gets or sets the OpenHAB widget
        /// </summary>
        public OpenHABWidget Widget
        {
            get => (OpenHABWidget)GetValue(WidgetProperty);
            set => SetValue(WidgetProperty, value);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
