using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using openHAB.Core.Client.Contracts;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;
using openHAB.Core.Common;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Core.Services;
using openHAB.Core.Services.Contracts;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// Collects and formats all the data for starting the application.
    /// </summary>
    public class MainViewModel : ViewModelBase<object>
    {
        private readonly IOpenHABClient _openHABClient;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<MainViewModel> _logger;
        private readonly SitemapService _sitemapManager;

        private ObservableCollection<OpenHABWidget> _breadcrumbItems;
        private bool _isDataLoading;
        private object _selectedMenuItem;
        private SitemapViewModel _selectedSitemap;
        private ObservableCollection<SitemapViewModel> _sitemaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="openHABClient">The OpenHAB client.</param>
        /// <param name="settingsService">Setting service instance.</param>
        /// <param name="logger">Logger class instance.</param>
        public MainViewModel(IOpenHABClient openHABClient, ISettingsService settingsService, SitemapService sitemapManager, ILogger<MainViewModel> logger)
            : base(new object())
        {
            _logger = logger;

            _openHABClient = openHABClient;
            _settingsService = settingsService;
            _breadcrumbItems = new ObservableCollection<OpenHABWidget>();
            _sitemapManager = sitemapManager;

            StrongReferenceMessenger.Default.Register<DataOperation>(this, (obj, operation)
                => DataOperationStateAsync(operation));

            StrongReferenceMessenger.Default.Register<WigetNavigation>(this, (obj, operation)
                => WidgetNavigatedEvent());
        }

        /// <summary>
        /// Gets or sets the items for the breadcrumb.
        /// </summary>
        /// <value>The breadcrumb items.</value>
        public ObservableCollection<OpenHABWidget> BreadcrumbItems
        {
            get => _breadcrumbItems;
            set => Set(ref _breadcrumbItems, value);
        }

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

        /// <summary>
        /// Gets or sets the selected menu item.
        /// </summary>
        public object SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }

            set
            {
                SitemapViewModel sitemapViewModel = value as SitemapViewModel;
                if (sitemapViewModel != null && SelectedSitemap != value)
                {
                    SelectedSitemap = sitemapViewModel;
                }

                Set(ref _selectedMenuItem, value);
            }
        }

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
                        Settings settings = _settingsService.Load();
                        settings.LastSitemap = _selectedSitemap.Name;
                        _settingsService.Save(settings);
                    }

                    if (!_isDataLoading && (_selectedSitemap?.Widgets == null || _selectedSitemap.Widgets.Count == 0))
                    {
                        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                        dispatcherQueue.EnqueueAsync(() =>
                        {
                            _selectedSitemap.LoadWidgets();
                            WidgetNavigationService.ClearWidgetNavigation();
                        });
                    }
                    else
                    {
                        _selectedSitemap?.SetWidgetsOnScreen(SelectedSitemap.Widgets);
                    }

                    SelectedMenuItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a collection of OpenHAB sitemaps.
        /// </summary>
        public ObservableCollection<SitemapViewModel> Sitemaps
        {
            get => _sitemaps;
            set => Set(ref _sitemaps, value);
        }

        #region Sitemaps Refresh Command

        private ActionCommand _refreshCommand;

        /// <summary>Gets the command to refresh sitemap and widget data.</summary>
        /// <value>The refresh command.</value>
        public ActionCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new ActionCommand(ExecuteRefreshCommandAsync, CanExecuteRefreshCommand));

        private bool CanExecuteRefreshCommand(object arg)
        {
            return !IsDataLoading;
        }

        private async void ExecuteRefreshCommandAsync(object obj)
        {
            await LoadSitemapsAndItemData().ConfigureAwait(false);
        }

        #endregion

        #region Load sitemap and Data

        /// <summary>
        /// Loads the sitemap data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task LoadSitemapsAndItemData()
        {
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                await App.DispatcherQueue.EnqueueAsync(async () =>
                {
                    await LoadData(cancellationTokenSource.Token).ConfigureAwait(false);
                });
            }
        }

        private async void CancelSyncCallbackAsync()
        {
            //await App.DispatcherQueue.EnqueueAsync(() =>
            //{
                Sitemaps?.Clear();
                SelectedSitemap.CurrentWidgets?.Clear();
                IsDataLoading = false;
            //});
        }

        private async Task LoadData(CancellationToken loadCancellationToken)
        {
            loadCancellationToken.Register(CancelSyncCallbackAsync);

            try
            {
                if (IsDataLoading)
                {
                    return;
                }

                _logger.LogInformation("Load available sitemaps and their items");

                IsDataLoading = true;
                Sitemaps?.Clear();

                if (loadCancellationToken.IsCancellationRequested)
                {
                    return;
                }

                List<OpenHABSitemap> sitemaps = await _sitemapManager.GetSitemaps(loadCancellationToken);
                if (sitemaps == null)
                {
                    StrongReferenceMessenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                    return;
                }

                List<SitemapViewModel> sitemapViewModels = new List<SitemapViewModel>();
                sitemaps.ToList().ForEach(model => sitemapViewModels.Add(new SitemapViewModel(model)));


                Sitemaps = new ObservableCollection<SitemapViewModel>(sitemapViewModels);
                _openHABClient.StartItemUpdates(loadCancellationToken);

                OpenLastOrDefaultSitemap();

                if (SelectedSitemap != null)
                {
                    await SelectedSitemap.SelectWidget().ConfigureAwait(false);
                    SelectedSitemap.ReloadSitemapCommand.InvokeCanExecuteChanged(null);
                }
            }
            catch (OpenHABException ex)
            {
                _logger.LogError(ex, "Load data failed.");
                StrongReferenceMessenger.Default.Send(new ConnectionErrorMessage(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load data failed.");
            }
            finally
            {
                IsDataLoading = false;
                RefreshCommand.InvokeCanExecuteChanged(null);
            }
        }

        private void OpenLastOrDefaultSitemap()
        {
            Settings settings = _settingsService.Load();
            string sitemapName = settings.LastSitemap;

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

        #region Events

        private async Task DataOperationStateAsync(DataOperation operation)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                switch (operation.State)
                {
                    case OperationState.Started:
                        IsDataLoading = true;
                        break;

                    case OperationState.Completed:
                        IsDataLoading = false;
                        break;
                }
            });
        }

        private void WidgetNavigatedEvent()
        {
            BreadcrumbItems?.Clear();
            BreadcrumbItems?.AddRange(WidgetNavigationService.Widgets);
            OnPropertyChanged(nameof(BreadcrumbItems));
        }

        #endregion
    }
}