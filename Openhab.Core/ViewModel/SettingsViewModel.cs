using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using GalaSoft.MvvmLight.Views;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using Windows.ApplicationModel;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for user defined settings.
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IOpenHAB _openHabsdk;
        private ConfigurationViewModel _configuration;

        private ICommand _localUrlCheckCommand;
        private OpenHABUrlState _localUrlState = OpenHABUrlState.Unknown;
        private ICommand _remoteUrlCheckCommand;
        private OpenHABUrlState _remoteUrlState = OpenHABUrlState.Unknown;
        private ICommand _saveCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel(ConfigurationViewModel configurationViewModel, INavigationService navigationService, IOpenHAB openHabsdk)
        {
            MessengerInstance.Register<PersistSettingsMessage>(this, msg => PersistSettings());

            _configuration = configurationViewModel;
            _navigationService = navigationService;
            _openHabsdk = openHabsdk;

            UrlChecks();
        }

        /// <summary>
        /// Gets the command for local url check.
        /// </summary>
        /// <value>The local URL check command.</value>
        public ICommand LocalUrlCheckCommand => _localUrlCheckCommand ?? (_localUrlCheckCommand = new RelayCommand<object>(CheckLocalUrl));

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
        /// Gets the command for remote url check.
        /// </summary>
        /// <value>The remote URL check command.</value>
        public ICommand RemoteUrlCheckCommand => _remoteUrlCheckCommand ?? (_remoteUrlCheckCommand = new RelayCommand(CheckRemoteUrl));

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
        /// Gets the save command to persist the settings.
        /// </summary>
        /// <value>The save command.</value>
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(PersistSettings));

        /// <summary>
        /// Gets or sets the current user-defined settings.
        /// </summary>
        public ConfigurationViewModel Settings
        {
            get => _configuration;
            set => Set(ref _configuration, value);
        }

        /// <summary>
        /// Gets the app version number.
        /// </summary>
        /// <value>The version number.</value>
        public string Version
        {
            get
            {
                Version version = new Version(
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);

                return version.ToString();
            }
        }

        /// <summary>
        /// Save the user defined settings to the UWP settings storage.
        /// </summary>
        public void PersistSettings()
        {
            _configuration.Save();

            MessengerInstance.Send(new SettingsUpdatedMessage());
            _navigationService.GoBack();
        }

        private async void CheckLocalUrl(object parameter)
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

        private async void CheckRemoteUrl()
        {
            if (string.IsNullOrEmpty(_configuration?.RemoteConnection?.Url) &&
                string.IsNullOrEmpty(_configuration?.RemoteConnection?.Username) &&
                string.IsNullOrEmpty(_configuration?.RemoteConnection?.Password))
            {
                return;
            }

            RemoteUrlState = OpenHABUrlState.Unknown;
            if (await _openHabsdk.CheckUrlReachability(Settings.RemoteConnection.Url, Common.OpenHABHttpClientType.Remote))
            {
                RemoteUrlState = OpenHABUrlState.OK;
            }
            else
            {
                RemoteUrlState = OpenHABUrlState.Failed;
            }
        }

        private void UrlChecks()
        {
            CheckLocalUrl(_configuration.LocalConnection.Url);
            CheckRemoteUrl();
        }
    }
}