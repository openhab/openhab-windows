using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Services.Store.Engagement;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the app.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IOpenHAB _openHabsdk;
        private readonly ISettingsService _settingsService;
        private ObservableCollection<OpenHABSitemap> _sitemaps;
        private OpenHABSitemap _selectedSitemap;
        private OpenHABVersion _version;
        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private OpenHABWidget _selectedWidget;
        private string _errorMessage;
        private string _subtitle;
        private readonly StoreServicesFeedbackLauncher _feedbackLauncher;

        private ICommand _feedbackCommand;
        private bool _isDataLoading;
        private readonly StoreServicesFeedbackLauncher _feedbackLauncher;

        /// <summary>
        /// Gets or sets an error message to show on screen.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        /// <summary>
        /// Gets or sets the subtitle of the page.
        /// </summary>
        public string Subtitle
        {
            get => _subtitle;
            set => Set(ref _subtitle, value);
        }

        /// <summary>
        /// Gets or sets a collection of OpenHAB sitemaps.
        /// </summary>
        public ObservableCollection<OpenHABSitemap> Sitemaps
        {
            get => _sitemaps;
            set => Set(ref _sitemaps, value);
        }

        /// <summary>
        /// Gets or sets the sitemap currently selected by the user.
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
                    if (_selectedSitemap != null)
                    {
                        _settingsService.SaveCurrentSitemap(_selectedSitemap.Name);
                    }

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
        /// Gets or sets the widgets currently on screen.
        /// </summary>
        public ObservableCollection<OpenHABWidget> CurrentWidgets
        {
            get => _currentWidgets;
            set => Set(ref _currentWidgets, value);
        }

        /// <summary>
        /// Gets or sets the selected widget.
        /// </summary>
        public OpenHABWidget SelectedWidget
        {
            get => _selectedWidget;
            set => Set(ref _selectedWidget, value);
        }


        /// <summary>Gets the command to open feedback app.</summary>
        /// <value>The feedback command.</value>
        public ICommand FeedbackCommand => _feedbackCommand ?? (_feedbackCommand = new RelayCommand(ExecuteFeedbackCommand, CanExecuteFeedbackCommand));

        private bool CanExecuteFeedbackCommand()
        {
            return StoreServicesFeedbackLauncher.IsSupported();
        }

        private async void ExecuteFeedbackCommand()
        {
            await _feedbackLauncher.LaunchAsync();
        }

        /// <summary>
        /// Gets or sets a value indicating whether data is loaded from an OpenHAB instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if data wil loaded; otherwise, <c>false</c>.</value>
        public bool IsDataLoading
        {
            get => _isDataLoading;
            set => Set(ref _isDataLoading, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="openHabsdk">The OpenHAB SDK object.</param>
        /// <param name="settingsService">Setting service instance.</param>
        public MainViewModel(IOpenHAB openHabsdk, ISettingsService settingsService)
        {
            ErrorMessage = "Test";
            IsDataLoading = false;
            CurrentWidgets = new ObservableCollection<OpenHABWidget>();

            _openHabsdk = openHabsdk;
            _settingsService = settingsService;
            _feedbackLauncher = StoreServicesFeedbackLauncher.GetDefault();

            MessengerInstance.Register<SettingsUpdatedMessage>(this, async msg =>
            {
                try
                {
                    ErrorMessage = "Invalid URL, check your settings";
                    if (await _openHabsdk.ResetConnection())
                    {
                        await LoadData();
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessengerInstance.Send(new FireErrorMessage(ex.Message));
                }
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

        public async Task LoadData()
        {
            try
            {
                if (IsDataLoading)
                {
                    return;
                }

                IsDataLoading = true;
                Sitemaps?.Clear();
                CurrentWidgets?.Clear();
                Subtitle = null;

                bool isSuccessful = await _openHabsdk.ResetConnection();
                if (!isSuccessful)
                {
                    MessengerInstance.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                _version = await _openHabsdk.GetOpenHABVersion();
                if (_version == OpenHABVersion.None)
                {
                    MessengerInstance.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                Func<OpenHABSitemap, bool> defaultSitemapFilter = (sitemap) =>
                {
                    return !sitemap.Name.Equals("_default", StringComparison.InvariantCultureIgnoreCase);
                };

                List<Func<OpenHABSitemap, bool>> filters = new List<Func<OpenHABSitemap, bool>>();

                Settings settings = _settingsService.Load();
                if (settings.HideDefaultSitemap.HasValue && settings.HideDefaultSitemap.Value)
                {
                    filters.Add(defaultSitemapFilter);
                }

                var sitemaps = await _openHabsdk.LoadSiteMaps(_version, filters);

                Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
                _openHabsdk.StartItemUpdates();

                OpenLastOrDefaultSitemap();
            }
            catch (OpenHABException ex)
            {
                MessengerInstance.Send(new FireErrorMessage(ex.Message));
            }
            catch (Exception ex)
            {
            
            }
            finally
            {
                IsDataLoading = false;
            }
        }

        private void OpenLastOrDefaultSitemap()
        {
            string sitemapName = _settingsService.LoadLastSitemap();

            if (string.IsNullOrWhiteSpace(sitemapName))
            {
                SelectedSitemap = Sitemaps.FirstOrDefault();
                return;
            }

            SelectedSitemap = Sitemaps.FirstOrDefault(x => x.Name == sitemapName);
            if (SelectedSitemap == null)
            {
                SelectedSitemap = Sitemaps.FirstOrDefault();
            }
        }

        private async Task LoadWidgets()
        {
            if (SelectedSitemap == null)
            {
                return;
            }

            CurrentWidgets.Clear();
            IsDataLoading = true;

            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version);
            SetWidgetsOnScreen(SelectedSitemap.Widgets);

            IsDataLoading = false;
        }

        private void OnWidgetClicked(OpenHABWidget widget)
        {
            SelectedWidget = widget;
            if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
            {
                return;
            }

            Subtitle = SelectedWidget.Label;
            WidgetNavigationService.Navigate(SelectedWidget);
            SetWidgetsOnScreen(SelectedWidget?.LinkedPage?.Widgets);
        }

        /// <summary>
        /// Navigate backwards between linkedpages.
        /// </summary>
        public void WidgetGoBack()
        {
            OpenHABWidget widget = WidgetNavigationService.GoBack();

            Subtitle = widget == null ? string.Empty : widget.Label;
            SetWidgetsOnScreen(widget != null ? widget.LinkedPage.Widgets : SelectedSitemap.Widgets);
        }

        private void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            CurrentWidgets.Clear();
            CurrentWidgets.AddRange(widgets);
        }
    }
}