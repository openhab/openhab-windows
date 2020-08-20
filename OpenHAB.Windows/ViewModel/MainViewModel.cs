using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Services.Store.Engagement;
using OpenHAB.Core;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the application.
    /// </summary>
    public class MainViewModel : ViewModelBase<object>
    {
        private readonly StoreServicesFeedbackLauncher _feedbackLauncher;
        private readonly IOpenHAB _openHabsdk;
        private readonly ISettingsService _settingsService;
        private CancellationTokenSource _cancellationTokenSource;
        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private string _errorMessage;
        private ICommand _feedbackCommand;
        private bool _isDataLoading;
        private ILogger<MainViewModel> _logger;
        private ActionCommand _refreshCommand;
        private SitemapViewModel _selectedSitemap;
        private OpenHABWidget _selectedWidget;
        private ObservableCollection<SitemapViewModel> _sitemaps;
        private string _subtitle;
        private OpenHABVersion _version;

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
            CurrentWidgets = new ObservableCollection<OpenHABWidget>();

            _openHabsdk = openHabsdk;
            _settingsService = settingsService;
            _feedbackLauncher = StoreServicesFeedbackLauncher.GetDefault();
            _cancellationTokenSource = new CancellationTokenSource();

            Messenger.Default.Register<TriggerCommandMessage>(this, async msg => await TriggerCommand(msg).ConfigureAwait(false));
            Messenger.Default.Register<WidgetClickedMessage>(this, msg => OnWidgetClicked(msg.Widget));
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
        /// Gets or sets an error message to show on screen.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        /// <summary>Gets the command to open feedback application.</summary>
        /// <value>The feedback command.</value>
        public ICommand FeedbackCommand => _feedbackCommand ?? (_feedbackCommand = new ActionCommand(ExecuteFeedbackCommand, CanExecuteFeedbackCommand));

        /// <summary>
        /// Gets or sets a value indicating whether data is loaded from an OpenHAB instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if data will loaded; otherwise, <c>false</c>.</value>
        public bool IsDataLoading
        {
            get => _isDataLoading;
            set => Set(ref _isDataLoading, value);
        }

        /// <summary>Gets the command to refresh sitemap and widget data.</summary>
        /// <value>The refresh command.</value>
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new ActionCommand(ExecuteRefreshCommandAsync, CanExecuteRefreshCommand));

        /// <summary>
        /// Gets or sets the sitemap currently selected by the user.
        /// </summary>
        public SitemapViewModel SelectedSitemap
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

                    if (_selectedSitemap?.Widgets == null || _selectedSitemap?.Widgets.Count == 0)
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
        /// Gets or sets the selected widget.
        /// </summary>
        public OpenHABWidget SelectedWidget
        {
            get => _selectedWidget;
            set => Set(ref _selectedWidget, value);
        }

        /// <summary>
        /// Gets or sets a collection of OpenHAB sitemaps.
        /// </summary>
        public ObservableCollection<SitemapViewModel> Sitemaps
        {
            get => _sitemaps;
            set => Set(ref _sitemaps, value);
        }

        /// <summary>
        /// Gets or sets the subtitle of the page.
        /// </summary>
        public string Subtitle
        {
            get => _subtitle;
            set => Set(ref _subtitle, value);
        }

        #region Commands

        private bool CanExecuteFeedbackCommand(object obj)
        {
            return StoreServicesFeedbackLauncher.IsSupported();
        }

        private bool CanExecuteRefreshCommand(object arg)
        {
            return !IsDataLoading;
        }

        private async void ExecuteFeedbackCommand(object obj)
        {
            await _feedbackLauncher.LaunchAsync();
        }

        private async void ExecuteRefreshCommandAsync(object obj)
        {
            await LoadSitemapsAndItemData().ConfigureAwait(false);
        }

        private async Task TriggerCommand(TriggerCommandMessage message)
        {
            await _openHabsdk.SendCommand(message.Item, message.Command).ConfigureAwait(false);
        }

        #endregion

        #region Load sitemap and Data

        /// <summary>
        /// Loads the sitemap data.
        /// </summary>
        public async Task LoadSitemapsAndItemData()
        {
            await LoadData(_cancellationTokenSource.Token).ConfigureAwait(false);
        }

        private async void CancelSyncCallbackAsync()
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Sitemaps?.Clear();
                CurrentWidgets?.Clear();
                Subtitle = null;
                IsDataLoading = false;
            });
        }

        private async Task LoadData(CancellationToken cancellationToken)
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            cancellationToken.Register(CancelSyncCallbackAsync);

            try
            {
                if (IsDataLoading)
                {
                    return;
                }

                _logger.LogInformation("Load available sitemap's and their items");

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsDataLoading = true;
                    Sitemaps?.Clear();
                    CurrentWidgets?.Clear();
                    Subtitle = null;
                });

                Settings settings = _settingsService.Load();
                if (settings.LocalConnection == null && settings.RemoteConnection == null)
                {
                    Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                }

                bool isSuccessful = await _openHabsdk.ResetConnection().ConfigureAwait(false);
                if (!isSuccessful)
                {
                    return;
                }

                _version = await _openHabsdk.GetOpenHABVersion().ConfigureAwait(false);
                if (_version == OpenHABVersion.None)
                {
                    Messenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                List<SitemapViewModel> sitemapViewModels = await LoadSitemaps(settings).ConfigureAwait(false);

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Sitemaps = new ObservableCollection<SitemapViewModel>(sitemapViewModels);
                    _openHabsdk.StartItemUpdates(cancellationToken);

                    OpenLastOrDefaultSitemap();

                    Subtitle = SelectedSitemap.Label;
                    IsDataLoading = false;
                });
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
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    IsDataLoading = false;
                });
            }
        }

        private async Task<List<SitemapViewModel>> LoadSitemaps(Settings settings)
        {
            Func<OpenHABSitemap, bool> defaultSitemapFilter = (sitemap) =>
            {
                return !sitemap.Name.Equals("_default", StringComparison.InvariantCultureIgnoreCase);
            };

            List<Func<OpenHABSitemap, bool>> filters = new List<Func<OpenHABSitemap, bool>>();

            if (!settings.ShowDefaultSitemap)
            {
                filters.Add(defaultSitemapFilter);
            }

            ICollection<OpenHABSitemap> sitemaps = await _openHabsdk.LoadSiteMaps(_version, filters).ConfigureAwait(false);
            List<SitemapViewModel> sitemapViewModels = new List<SitemapViewModel>();
            sitemaps.ToList().ForEach(x => sitemapViewModels.Add(new SitemapViewModel(x)));

            return sitemapViewModels;
        }

        private void OpenLastOrDefaultSitemap()
        {
            string sitemapName = _settingsService.LoadLastSitemap();

            if (string.IsNullOrWhiteSpace(sitemapName))
            {
                _logger.LogInformation("No sitemap was selected in the past -> Pick first entry from list");

                SelectedSitemap = Sitemaps.FirstOrDefault();
                return;
            }

            SelectedSitemap = Sitemaps.FirstOrDefault(x => x.Name == sitemapName);
            if (SelectedSitemap == null)
            {
                _logger.LogInformation($"Unable to find sitemap '{sitemapName}' -> Pick first entry from list");
                SelectedSitemap = Sitemaps.FirstOrDefault();
            }
        }

        #endregion

        #region Widget Handing

        /// <summary>
        /// Navigate backwards between linked pages.
        /// </summary>
        public void WidgetGoBack()
        {
            OpenHABWidget widget = WidgetNavigationService.GoBack();

            Subtitle = widget == null ? SelectedSitemap?.Label : widget.Label;
            SetWidgetsOnScreen(widget != null ? widget.LinkedPage.Widgets : SelectedSitemap.Widgets);
        }

        private async Task LoadWidgets()
        {
            if (SelectedSitemap == null)
            {
                return;
            }

            CurrentWidgets.Clear();
            IsDataLoading = true;

            await SelectedSitemap.LoadWidgets(_version).ConfigureAwait(false);
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

        private async void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            CoreDispatcher dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                CurrentWidgets.Clear();
                CurrentWidgets.AddRange(widgets);
            });
        }

        #endregion
    }
}