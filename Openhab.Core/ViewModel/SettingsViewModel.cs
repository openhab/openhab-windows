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
        private ConfigurationViewModel _configuration;
        private ICommand _saveCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel(ConfigurationViewModel configurationViewModel, INavigationService navigationService)
        {
            MessengerInstance.Register<PersistSettingsMessage>(this, msg => PersistSettings());

            _configuration = configurationViewModel;
            _navigationService = navigationService;
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
            if (_configuration.IsConnectionConfigValid())
            {
                _configuration.Save();
                _navigationService.GoBack();
            }
            else
            {
                MessengerInstance.Send(new SettingsUpdatedMessage());
            }
        }
    }
}