using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB switch.
    /// </summary>
    public sealed partial class SectionSwitchWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionSwitchWidget"/> class.
        /// </summary>
        public SectionSwitchWidget()
        {
            InitializeComponent();
            Loaded += SectionSwitchWidget_Loaded;
        }

        private void SectionSwitchWidget_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Widget.Item?.State))
            {
                return;
            }

            SetState();
        }

        internal override void SetState()
        {
            UncheckEverything();
            var currentItem = SectionsList?.Items?.SingleOrDefault(x => ((WidgetMapping)x).Command == Widget.Item.State);
            ContentPresenter presenter = SectionsList?.ContainerFromItem(currentItem) as Microsoft.UI.Xaml.Controls.ContentPresenter;

            if (presenter != null && presenter.FindChild(typeof(ToggleButton)) is ToggleButton toggleButton)
            {
                toggleButton.IsChecked = true;
            }
        }

        private void Button_OnClick(object sender, TappedRoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            if (button == null)
            {
                return;
            }

            if (string.CompareOrdinal(button?.Tag?.ToString(), Widget.Item.State) == 0)
            {
                SetState();
                return;
            }

            UncheckEverything();
            button.IsChecked = true;

            StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, button?.Tag.ToString()));
        }

        private void UncheckEverything()
        {
            foreach (var item in SectionsList.Items)
            {
                ContentPresenter presenter = SectionsList?.ContainerFromItem(item) as ContentPresenter;
                if (presenter.FindChild(typeof(ToggleButton)) is ToggleButton toggleButton)
                {
                    toggleButton.IsChecked = false;
                }
            }
        }
    }
}