using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using GalaSoft.MvvmLight.Views;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for user defined settings
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly IOpenHAB _openHabsdk;
        private Settings _settings;

        private ICommand _saveCommand;
        private ICommand _localUrlCheckCommand;
        private ICommand _remoteUrlCheckCommand;

        private OpenHABUrlState _localUrlState = OpenHABUrlState.Unknown;
        private OpenHABUrlState _remoteUrlState = OpenHABUrlState.Unknown;

        /// <summary>
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(PersistSettings));

        /// <summary>
        /// Gets the command for local url check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand LocalUrlCheckCommand => _localUrlCheckCommand ?? (_localUrlCheckCommand = new RelayCommand<object>(CheckLocalUrl));

        /// <summary>
        /// Gets the command for remote url check.
        /// </summary>
        /// <value>The remote URL check command.</value>
        public ICommand RemoteUrlCheckCommand => _remoteUrlCheckCommand ?? (_remoteUrlCheckCommand = new RelayCommand<object>(CheckRemoteUrl));

        /// <summary>
        /// Gets or sets the current user-defined settings
        /// </summary>
        public Settings Settings
        {
            get => _settings;
            set => Set(ref _settings, value);
        }

        /// <summary>
        /// Gets or sets the state for OpenHab local url.
        /// </summary>
        /// <value>The state of the local URL.</value>
        public OpenHABUrlState LocalUrlState
        {
            get => _localUrlState;
            set => Set(ref _localUrlState, value);
        }

        /// <summary>
        /// Gets or sets the state for OpenHab remote url.
        /// </summary>
        /// <value>The state of the remote URL.</value>
        public OpenHABUrlState RemoteUrlState
        {
            get => _remoteUrlState;
            set => Set(ref _remoteUrlState, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel(ISettingsService settingsService, INavigationService navigationService, IOpenHAB openHabsdk)
        {
            MessengerInstance.Register<PersistSettingsMessage>(this, msg => PersistSettings());

            _settingsService = settingsService;
            _navigationService = navigationService;
            _openHabsdk = openHabsdk;

            LoadSettings();

            UrlChecks();
        }

        /// <summary>
        /// Save the user defined settings to the UWP settings storage.
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

        private void UrlChecks()
        {
            CheckLocalUrl(_settings.OpenHABUrl);
            CheckRemoteUrl(_settings.OpenHABRemoteUrl);
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        private async void CheckLocalUrl(object parameter)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            LocalUrlState = OpenHABUrlState.Unknown;
            if (await _openHabsdk.CheckUrlReachability(url, Common.OpenHABHttpClientType.Local))
            {
                LocalUrlState = OpenHABUrlState.OK;
            }
            else
            {
                LocalUrlState = OpenHABUrlState.Failed;
            }
        }

#pragma warning disable S3168 // "async" methods should not return "void"
        private async void CheckRemoteUrl(object parameter)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            if (parameter == null)
            {
                return;
            }

            string url = parameter.ToString();

            RemoteUrlState = OpenHABUrlState.Unknown;
            if (await _openHabsdk.CheckUrlReachability(url, Common.OpenHABHttpClientType.Remote))
            {
                RemoteUrlState = OpenHABUrlState.OK;
            }
            else
            {
                RemoteUrlState = OpenHABUrlState.Failed;
            }
        }
    }
}