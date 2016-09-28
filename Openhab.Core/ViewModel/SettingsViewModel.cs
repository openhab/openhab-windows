using GalaSoft.MvvmLight;
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
        private Settings _settings;

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
        public SettingsViewModel(ISettingsService settingsService)
        {
            MessengerInstance.Register<PersistSettingsMessage>(this, msg => PersistSettings());

            _settingsService = settingsService;
            LoadSettings();
        }

        private void PersistSettings()
        {
            _settingsService.Save(Settings);

            MessengerInstance.Send(new SettingsUpdatedMessage());
        }

        private void LoadSettings()
        {
            Settings = _settingsService.Load();
        }
    }
}