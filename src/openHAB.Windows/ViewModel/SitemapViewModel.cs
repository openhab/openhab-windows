using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using openHAB.Common;
using openHAB.Core.Client.Common;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;
using openHAB.Core.Common;
using openHAB.Core.Messages;
using openHAB.Core.Services;
using openHAB.Windows.Services;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel class for the sitemap view.
    /// </summary>
    public class SitemapViewModel : ViewModelBase<OpenHABSitemap>, IDisposable
    {
        private readonly SitemapService _sitemapService;

        private ObservableCollection<WidgetViewModel> _currentWidgets;
        private WidgetViewModel _selectedWidget;
        private ObservableCollection<WidgetViewModel> _widgets;
        private bool disposedValue;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
        /// </summary>
        public SitemapViewModel()
            : base(new OpenHABSitemap())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
        /// </summary>
        /// <param name="model">Model class for view model.</param>
        /// <param name="serverInfo">openHAB Instance information.</param>
        public SitemapViewModel(OpenHABSitemap model)
             : base(model)
        {
            List<WidgetViewModel> widgetViewModels = GetWidgetViewModels(model.Widgets).Result;
            _widgets = new ObservableCollection<WidgetViewModel>(widgetViewModels ?? new List<WidgetViewModel>());
            _sitemapService = DIService.Instance.GetService<SitemapService>();
            _currentWidgets = new ObservableCollection<WidgetViewModel>();

            StrongReferenceMessenger.Default.Register<WidgetClickedMessage>(this, async (recipient, msg)
                =>
            {
                if (msg.Widget == null)
                {
                    return;
                }

                await OnWidgetClickedAsync(new WidgetViewModel(msg.Widget));
            });

            StrongReferenceMessenger.Default.Register<TriggerCommandMessage>(this, async (recipient, msg)
                => await TriggerItemCommand(msg).ConfigureAwait(false));

            StrongReferenceMessenger.Default.Register<DataOperation>(this, (obj, operation)
                => DataOperationState(operation));

            StrongReferenceMessenger.Default.Register<WigetNavigation>(this, (recipient, msg) =>
            {
                if (msg.Trigger == EventTriggerSource.Breadcrumb)
                {
                    WidgetGoBack(msg.TargetWidget);
                }
                else if (msg.Trigger == EventTriggerSource.Root)
                {
                    ExecuteNavigateToSitemapRootCommand(null);
                }
            });

            SetWidgetsOnScreen(Widgets);
        }

        private async Task<List<WidgetViewModel>> GetWidgetViewModels(ICollection<OpenHABWidget> widgets)
        {

            List<WidgetViewModel> widgetViewModels = new List<WidgetViewModel>();
            foreach (OpenHABWidget widget in widgets)
            {
                WidgetViewModel viewModel = new WidgetViewModel(widget);
                widgetViewModels.Add(viewModel);
            }

            return widgetViewModels;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the widgets currently on screen.
        /// </summary>
        public ObservableCollection<WidgetViewModel> CurrentWidgets
        {
            get => _currentWidgets;
            set => Set(ref _currentWidgets, value);
        }

        /// <summary>
        /// Gets the label of the OpenHAB sitemap.
        /// </summary>
        public string Label
        {
            get
            {
                return Model.Label;
            }
        }

        /// <summary>
        /// Gets the link of the OpenHAB sitemap.
        /// </summary>
        public string Link
        {
            get
            {
                return Model.Link;
            }
        }

        /// <summary>
        /// Gets the name of the OpenHAB sitemap.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        /// <summary>
        /// Gets or sets the selected widget.
        /// </summary>
        public WidgetViewModel SelectedWidget
        {
            get => _selectedWidget;
            set => Set(ref _selectedWidget, value);
        }

        /// <summary>
        /// Gets or sets a collection of widgets of the OpenHAB sitemap.
        /// </summary>
        public ObservableCollection<WidgetViewModel> Widgets
        {
            get
            {
                return _widgets;
            }

            set
            {
                if (Equals(value, _widgets))
                {
                    return;
                }

                this.Set(ref _widgets, value);
            }
        }

        #endregion

        #region Reload Command

        private ActionCommand _reloadSitemapCommand;

        /// <summary>Gets the command to reload sitemap and widget data.</summary>
        /// <value>The refresh command.</value>
        public ActionCommand ReloadSitemapCommand => _reloadSitemapCommand ?? (_reloadSitemapCommand = new ActionCommand(ExecuteReloadSitemapCommand, CanExecuteReloadSitemapCommand));

        private bool CanExecuteReloadSitemapCommand(object arg)
        {
            return !_canExecuteReloadSitemap;
        }

        private async void ExecuteReloadSitemapCommand(object obj)
        {
            await ReloadSitemap().ConfigureAwait(false);
        }

        #endregion

        #region Navigate To Sitemap Root Command

        private bool _canExecuteReloadSitemap;
        private ActionCommand _navigateToSitemapRootCommand;

        public ActionCommand NavigateToSitemapRoot =>
            _navigateToSitemapRootCommand ?? (_navigateToSitemapRootCommand = new ActionCommand(ExecuteNavigateToSitemapRootCommand, CanExecuteNavigateToSitemapRootCommand));

        private bool CanExecuteNavigateToSitemapRootCommand(object arg)
        {
            return true;
        }

        private void ExecuteNavigateToSitemapRootCommand(object obj)
        {
            WidgetNavigationService.ClearWidgetNavigation();
            SetWidgetsOnScreen(Widgets);
            SelectedWidget = null;

            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(SelectedWidget.Model, null, EventTriggerSource.Widget));
        }

        #endregion

        #region Events

        private void DataOperationState(DataOperation operation)
        {
            switch (operation.State)
            {
                case OperationState.Started:
                    _canExecuteReloadSitemap = true;
                    break;
                case OperationState.Completed:
                    _canExecuteReloadSitemap = false;
                    break;
            }
        }

        private async Task TriggerItemCommand(TriggerCommandMessage message)
        {
            HttpResponseResult<bool> result = await _sitemapService.SendItemCommand(message.Item, message.Command).ConfigureAwait(false);
            if (!result.Content)
            {
                string errorMessage = AppResources.Errors.GetString("CommandFailed");
                errorMessage = string.Format(errorMessage, message.Command, message.Item?.Name);

                StrongReferenceMessenger.Default.Send<ConnectionErrorMessage>(new ConnectionErrorMessage(errorMessage));
            }
        }

        #endregion

        private async Task LoadWidgetsAsync()
        {
            this.Widgets = new ObservableCollection<WidgetViewModel>();
            CurrentWidgets?.Clear();

            ICollection<OpenHABWidget> widgetModels = await _sitemapService.LoadItemsFromSitemapAsync(Model).ConfigureAwait(false);
            widgetModels.ToList().ForEach(model => Widgets.Add(new WidgetViewModel(model)));

            SetWidgetsOnScreen(this.Widgets);
        }

        private async Task ReloadSitemap()
        {
            CurrentWidgets?.Clear();
            StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

            if (SelectedWidget != null)
            {
                await LoadWidgetsAsync().ConfigureAwait(false);
                WidgetViewModel widget = FindWidget(SelectedWidget.WidgetId, Widgets);
                if (widget != null)
                {
                    await OnWidgetClickedAsync(widget);
                }
                else
                {
                    SelectedWidget = null;
                    WidgetNavigationService.ClearWidgetNavigation();
                }
            }
            else
            {
                await LoadWidgetsAsync().ConfigureAwait(false);
            }

            StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));
            ReloadSitemapCommand.InvokeCanExecuteChanged(null);
        }

        #region Widget interaction

        private WidgetViewModel FindWidget(string widgetId, ICollection<WidgetViewModel> widgets)
        {
            WidgetViewModel openHABWidget = null;
            if (widgets == null || widgets.Count == 0)
            {
                return openHABWidget;
            }

            foreach (WidgetViewModel widget in widgets)
            {
                if (string.CompareOrdinal(widget.WidgetId, widgetId) == 0)
                {
                    return widget;
                }

                ICollection<WidgetViewModel> childWidgets = widget.Type.CompareTo("Group") == 0 ? widget.LinkedPage?.Widgets : widget.Children;
                openHABWidget = FindWidget(widgetId, childWidgets);
                if (openHABWidget != null)
                {
                    return openHABWidget;
                }
            }

            return openHABWidget;
        }

        private async Task OnWidgetClickedAsync(WidgetViewModel widget)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                WidgetViewModel lastWidget = SelectedWidget;
                SelectedWidget = widget;
                if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
                {
                    return;
                }

                WidgetNavigationService.Navigate(SelectedWidget.Model);
                StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget.Model, widget.Model, EventTriggerSource.Widget));

                SetWidgetsOnScreen(SelectedWidget.LinkedPage.Widgets);
            });
        }

        private async void SetWidgetsOnScreen(ICollection<WidgetViewModel> widgets)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                CurrentWidgets.Clear();
                CurrentWidgets.AddRange(widgets);
            });
        }

        private void WidgetGoBack(OpenHABWidget widget)
        {
            WidgetViewModel lastWidget = SelectedWidget;
            WidgetViewModel widgetFromStack = null;

            while (widgetFromStack == null || widgetFromStack.WidgetId != widget.WidgetId)
            {
                widgetFromStack = new WidgetViewModel(WidgetNavigationService.GoBack());
            }

            SelectedWidget = widgetFromStack;
            WidgetNavigationService.Navigate(SelectedWidget.Model);
            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget.Model, SelectedWidget.Model, EventTriggerSource.Widget));

            SetWidgetsOnScreen(SelectedWidget.LinkedPage.Widgets);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StrongReferenceMessenger.Default.Unregister<WidgetClickedMessage>(this);
                    StrongReferenceMessenger.Default.Unregister<TriggerCommandMessage>(this);
                    StrongReferenceMessenger.Default.Unregister<DataOperation>(this);
                    StrongReferenceMessenger.Default.Unregister<WigetNavigation>(this);

                    Widgets = null;
                    CurrentWidgets = null;
                    SelectedWidget = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        #endregion
    }
}