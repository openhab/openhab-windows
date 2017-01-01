using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for user defined settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private Settings _settings;
        private ICommand _saveCommand;

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(PersistSettings));

        /// <summary>
        /// Gets or sets the current user-defined settings
        /// </summary>
        public Settings Settings
        {
            get { return _settings; }
            set { Set(ref _settings, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel(ISettingsService settingsService, INavigationService navigationService)
        {
            MessengerInstance.Register<PersistSettingsMessage>(this, msg => PersistSettings());

            _settingsService = settingsService;
            _navigationService = navigationService;
            LoadSettings();
        }

        /// <summary>
        /// Save the user defined settings to the UWP settings storage
        /// </summary>
        public void PersistSettings()
        {
            _settingsService.Save(Settings);

            MessengerInstance.Send(new SettingsUpdatedMessage());
            _navigationService.GoBack();
        }

        private void LoadSettings()
        {
            Settings = _settingsService.Load();
        }
    }
}