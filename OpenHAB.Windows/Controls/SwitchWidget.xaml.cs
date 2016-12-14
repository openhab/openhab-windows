using GalaSoft.MvvmLight.Messaging;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch
    /// </summary>
    public sealed partial class SwitchWidget : WidgetBase
    {
        /// <summary>
        /// Property to bind the toggle state to
        /// </summary>
        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
            "IsOn", typeof(bool), typeof(SwitchWidget), new PropertyMetadata(default(bool), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((SwitchWidget)dependencyObject).OnToggle();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toggle is on or off
        /// </summary>
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchWidget"/> class.
        /// </summary>
        public SwitchWidget()
        {
            InitializeComponent();
        }

        private void OnToggle()
        {
            Messenger.Default.Send(new TriggerCommandMessage(Widget.Item, IsOn ? OpenHABCommands.OnCommand : OpenHABCommands.OffCommand));
        }

        private void OnToggle(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, ToggleStates.CurrentState == OnState ? "OffState" : "OnState", true);
        }
    }
}