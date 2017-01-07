using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the app
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IOpenHAB _openHabsdk;
        private ObservableCollection<OpenHABSitemap> _sitemaps;
        private OpenHABSitemap _selectedSitemap;
        private OpenHABVersion _version;
        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private OpenHABWidget _selectedWidget;

        /// <summary>
        /// Gets or sets a collection of OpenHAB sitemaps
        /// </summary>
        public ObservableCollection<OpenHABSitemap> Sitemaps
        {
            get { return _sitemaps; }
            set { Set(ref _sitemaps, value); }
        }

        /// <summary>
        /// Gets or sets the sitemap currently selected by the user
        /// </summary>
        public OpenHABSitemap SelectedSitemap
        {
            get
            {
                return _selectedSitemap;
            }

            set
            {
                if (Set(ref _selectedSitemap, value))
                {
                    if (_selectedSitemap?.Widgets == null)
                    {
#pragma warning disable 4014
                        LoadWidgets();
#pragma warning restore 4014
                    }
                    else
                    {
                        SetWidgetsOnScreen(SelectedSitemap.Widgets);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the widgets currently on screen
        /// </summary>
        public ObservableCollection<OpenHABWidget> CurrentWidgets
        {
            get { return _currentWidgets; }
            set { Set(ref _currentWidgets, value); }
        }

        /// <summary>
        /// Gets or sets the selected widget
        /// </summary>
        public OpenHABWidget SelectedWidget
        {
            get { return _selectedWidget; }
            set { Set(ref _selectedWidget, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="openHabsdk">The OpenHAB SDK object</param>
        public MainViewModel(IOpenHAB openHabsdk)
        {
            CurrentWidgets = new ObservableCollection<OpenHABWidget>();
            _openHabsdk = openHabsdk;

            MessengerInstance.Register<SettingsUpdatedMessage>(this, async msg =>
            {
                await _openHabsdk.ResetConnection();
                await LoadData();
            });

            MessengerInstance.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg));
            MessengerInstance.Register<WidgetClickedMessage>(this, msg => OnWidgetClicked(msg.Widget));
#pragma warning disable 4014
            LoadData();
#pragma warning restore 4014
        }

        private async Task TriggerCommand(TriggerCommandMessage message)
        {
            await _openHabsdk.SendCommand(message.Item, message.Command);
        }

        private async Task LoadData()
        {
            await _openHabsdk.ResetConnection();
            _version = await _openHabsdk.GetOpenHABVersion();

            if (_version == OpenHABVersion.None)
            {
                return;
            }

            var sitemaps = await _openHabsdk.LoadSiteMaps(_version);
            Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
        }

        private async Task LoadWidgets()
        {
            if (SelectedSitemap == null)
            {
                return;
            }

            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version);
            SetWidgetsOnScreen(SelectedSitemap.Widgets);
        }

        private void OnWidgetClicked(OpenHABWidget widget)
        {
            SelectedWidget = widget;
            if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
            {
                return;
            }

            WidgetNavigationService.Navigate(SelectedWidget);
            SetWidgetsOnScreen(SelectedWidget?.LinkedPage?.Widgets);
        }

        /// <summary>
        /// Navigate backwards between linkedpages
        /// </summary>
        public void WidgetGoBack()
        {
            OpenHABWidget widget = WidgetNavigationService.GoBack();
            SetWidgetsOnScreen(widget != null ? widget.LinkedPage.Widgets : SelectedSitemap.Widgets);
        }

        private void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            CurrentWidgets.Clear();
            CurrentWidgets.AddRange(widgets);
        }
    }
}