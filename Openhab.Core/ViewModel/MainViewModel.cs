using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
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
                    if (_selectedSitemap.Widgets == null)
                    {
#pragma warning disable 4014
                        LoadWidgets();
#pragma warning restore 4014
                    }
                }
            }
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
                _openHabsdk.ResetConnection();
                await LoadData();
            });
            MessengerInstance.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg));
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
            _version = await _openHabsdk.GetOpenHABVersion();
            var sitemaps = await _openHabsdk.LoadSiteMaps(_version);
            Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
        }

        private async Task LoadWidgets()
        {
            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version);
        }
    }
}