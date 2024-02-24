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

        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private OpenHABWidget _selectedWidget;
        private ObservableCollection<OpenHABWidget> _widgets;

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
            _widgets = new ObservableCollection<OpenHABWidget>(model.Widgets ?? new List<OpenHABWidget>());
            _sitemapService = DIService.Instance.GetService<SitemapService>();
            _currentWidgets = new ObservableCollection<OpenHABWidget>();

            StrongReferenceMessenger.Default.Register<WidgetClickedMessage>(this, async (recipient, msg)
                => await OnWidgetClickedAsync(msg.Widget));

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the widgets currently on screen.
        /// </summary>
        public ObservableCollection<OpenHABWidget> CurrentWidgets
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
        public OpenHABWidget SelectedWidget
        {
            get => _selectedWidget;
            set => Set(ref _selectedWidget, value);
        }

        /// <summary>
        /// Gets or sets a collection of widgets of the OpenHAB sitemap.
        /// </summary>
        public ObservableCollection<OpenHABWidget> Widgets
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
        private bool disposedValue;

        public ActionCommand NavigateToSitemapRoot => _navigateToSitemapRootCommand ?? (_navigateToSitemapRootCommand = new ActionCommand(ExecuteNavigateToSitemapRootCommand, CanExecuteNavigateToSitemapRootCommand));

        private bool CanExecuteNavigateToSitemapRootCommand(object arg)
        {
            return true;
        }

        private void ExecuteNavigateToSitemapRootCommand(object obj)
        {
            WidgetNavigationService.ClearWidgetNavigation();
            SetWidgetsOnScreen(Widgets);
            SelectedWidget = null;

            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(SelectedWidget, null, EventTriggerSource.Widget));
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

        private async void OnWidgetClickedAction(OpenHABWidget widget)
        {
            await OnWidgetClickedAsync(widget);
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

        /// <summary>
        /// Loads widgets for sitemap.
        /// </summary>W
        /// <returns>Task for async processing.</returns>
        public async Task LoadWidgetsAsync()
        {
            this.Widgets = new ObservableCollection<OpenHABWidget>();
            CurrentWidgets?.Clear();

            ICollection<OpenHABWidget> widgetModels = await _sitemapService.LoadItemsFromSitemapAsync(Model).ConfigureAwait(false);
            widgetModels.ToList().ForEach(x => Widgets.Add(x));

            SetWidgetsOnScreen(this.Widgets);
        }

        public async Task ReloadSitemap()
        {
            CurrentWidgets?.Clear();
            StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

            if (SelectedWidget != null)
            {
                await LoadWidgetsAsync().ConfigureAwait(false);
                OpenHABWidget widget = FindWidget(SelectedWidget.WidgetId, Widgets);
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

        //public async Task SelectWidget()
        //{
        //    if (SelectedWidget != null)
        //    {
        //        await LoadWidgetsAsync().ConfigureAwait(false);
        //        OpenHABWidget widget = FindWidget(SelectedWidget.WidgetId, Widgets);
        //        if (widget != null)
        //        {
        //            await OnWidgetClickedAsync(widget);
        //        }
        //        else
        //        {
        //            SelectedWidget = null;
        //            WidgetNavigationService.ClearWidgetNavigation();
        //        }
        //    }
        //    else
        //    {
        //        await LoadWidgetsAsync().ConfigureAwait(false);
        //    }
        //}

        public async void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                CurrentWidgets.Clear();
                CurrentWidgets.AddRange(widgets);
            });
        }

        /// <summary>
        /// Navigate backwards between linked pages.
        /// </summary>
        public void WidgetGoBack(OpenHABWidget widget)
        {
            OpenHABWidget lastWidget = SelectedWidget;
            OpenHABWidget widgetFromStack = null;

            while (widgetFromStack == null || widgetFromStack.WidgetId != widget.WidgetId)
            {
                widgetFromStack = WidgetNavigationService.GoBack();
            }

            SelectedWidget = widgetFromStack;
            WidgetNavigationService.Navigate(SelectedWidget);
            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget, SelectedWidget, EventTriggerSource.Widget));

            SetWidgetsOnScreen(SelectedWidget.LinkedPage.Widgets);
        }

        private OpenHABWidget FindWidget(string widgetId, ICollection<OpenHABWidget> widgets)
        {
            OpenHABWidget openHABWidget = null;
            if (widgets == null || widgets.Count == 0)
            {
                return openHABWidget;
            }

            foreach (OpenHABWidget widget in widgets)
            {
                if (string.CompareOrdinal(widget.WidgetId, widgetId) == 0)
                {
                    return widget;
                }

                ICollection<OpenHABWidget> childWidgets = widget.Type.CompareTo("Group") == 0 ? widget.LinkedPage?.Widgets : widget.Children;
                openHABWidget = FindWidget(widgetId, childWidgets);
                if (openHABWidget != null)
                {
                    return openHABWidget;
                }
            }

            return openHABWidget;
        }

        private async Task OnWidgetClickedAsync(OpenHABWidget widget)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                OpenHABWidget lastWidget = SelectedWidget;
                SelectedWidget = widget;
                if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
                {
                    return;
                }

                WidgetNavigationService.Navigate(SelectedWidget);
                StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget, widget, EventTriggerSource.Widget));

                SetWidgetsOnScreen(SelectedWidget.LinkedPage.Widgets);
            });
        }

        #region Dispose

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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}