using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Services.Store.Engagement;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Core.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the app.
    /// </summary>
    public class MainViewModel : ViewModelBase<object>
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
        private ILogger<MainViewModel> _logger;

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
        public ICommand FeedbackCommand => _feedbackCommand ?? (_feedbackCommand = new ActionCommand(ExecuteFeedbackCommand, CanExecuteFeedbackCommand));

        private bool CanExecuteFeedbackCommand(object obj)
        {
            return StoreServicesFeedbackLauncher.IsSupported();
        }

        private async void ExecuteFeedbackCommand(object obj)
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
        /// <param name="logger">Logger class instance.</param>
        public MainViewModel(IOpenHAB openHabsdk, ISettingsService settingsService, ILogger<MainViewModel> logger) 
            : base(new object())
        {
            _logger = logger;
            IsDataLoading = false;
            CurrentWidgets = new ObservableCollection<OpenHABWidget>();

            _openHabsdk = openHabsdk;
            _settingsService = settingsService;
            _feedbackLauncher = StoreServicesFeedbackLauncher.GetDefault();

            Messenger.Default.Register<SettingsUpdatedMessage>(this, async msg =>
            {
                try
                {
                    ErrorMessage = "Invalid URL, check your settings";
                    if (await _openHabsdk.ResetConnection().ConfigureAwait(false))
                    {
                        await LoadData().ConfigureAwait(false);
                    }
                }
                catch (HttpRequestException ex)
                {
                    Messenger.Default.Send(new FireErrorMessage(ex.Message));
                }
            });

            Messenger.Default.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg).ConfigureAwait(false));
            Messenger.Default.Register<WidgetClickedMessage>(this, msg => OnWidgetClicked(msg.Widget));

        }

        private async Task TriggerCommand(TriggerCommandMessage message)
        {
            await _openHabsdk.SendCommand(message.Item, message.Command).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the sitemap data.
        /// </summary>
        public async Task LoadData()
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
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

                    _logger.LogInformation("Load Data");

                    bool isSuccessful = await _openHabsdk.ResetConnection().ConfigureAwait(false);
                    if (!isSuccessful)
                    {
                        Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                        return;
                    }

                    _version = await _openHabsdk.GetOpenHABVersion().ConfigureAwait(false);
                    if (_version == OpenHABVersion.None)
                    {
                        Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
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

                    var sitemaps = await _openHabsdk.LoadSiteMaps(_version, filters).ConfigureAwait(false);

                    Sitemaps = new ObservableCollection<OpenHABSitemap>(sitemaps);
                    _openHabsdk.StartItemUpdates();

                    OpenLastOrDefaultSitemap();
                }
                catch (OpenHABException ex)
                {
                    _logger.LogError(ex, "Load data failed.");
                    Messenger.Default.Send(new FireErrorMessage(ex.Message));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Load data failed.");
                }
                finally
                {
                    IsDataLoading = false;
                }
            });
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

            SelectedSitemap.Widgets = await _openHabsdk.LoadItemsFromSitemap(SelectedSitemap, _version).ConfigureAwait(false);
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