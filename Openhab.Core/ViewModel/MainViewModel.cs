using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;

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
            var sitemaps = await _openHabsdk.LoadSiteMaps(_version);
            Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
        }

        private async Task LoadWidgets()
        {
            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version);
            CurrentWidgets = new ObservableCollection<OpenHABWidget>(SelectedSitemap.Widgets);
        }

        private void OnWidgetClicked(OpenHABWidget widget)
        {
            SelectedWidget = widget;
            if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
            {
                return;
            }

            CurrentWidgets.Clear();
            CurrentWidgets.AddRange(SelectedWidget?.LinkedPage?.Widgets);
        }
    }
}