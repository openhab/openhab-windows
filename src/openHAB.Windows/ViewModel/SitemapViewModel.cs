using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using openHAB.Core;
using openHAB.Core.Common;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Core.SDK;
using openHAB.Core.Services;
using openHAB.Windows.Services;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// A class that represents an OpenHAB sitemap.
    /// </summary>
    public class SitemapViewModel : ViewModelBase<OpenHABSitemap>
    {
        private ObservableCollection<OpenHABWidget> _currentWidgets;
        private IOpenHAB _openHabsdk;
        private OpenHABWidget _selectedWidget;
        private string _subtitle;
        private ServerInfo _serverInfo;
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
        public SitemapViewModel(OpenHABSitemap model, ServerInfo serverInfo)
             : base(model)
        {
            _serverInfo = serverInfo;
            _widgets = new ObservableCollection<OpenHABWidget>(model.Widgets ?? new List<OpenHABWidget>());
            _openHabsdk = DIService.Instance.GetService<IOpenHAB>();

            _currentWidgets = new ObservableCollection<OpenHABWidget>();

            StrongReferenceMessenger.Default.Register<WidgetClickedMessage>(this, (recipient, msg)
                => OnWidgetClickedAction(recipient, msg.Widget));

            StrongReferenceMessenger.Default.Register<TriggerCommandMessage>(this, async (recipient, msg)
                => await TriggerItemCommand(recipient, msg).ConfigureAwait(false));

            StrongReferenceMessenger.Default.Register<DataOperation>(this, (obj, operation)
                => DataOperationState(obj, operation));
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

                _widgets = value;
                OnPropertyChanged();
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

        private ActionCommand _navigateToSitemapRootCommand;
        private bool _canExecuteReloadSitemap;

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

            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(SelectedWidget, null));
        }

        #endregion

        #region Events

        private async void OnWidgetClickedAction(object recipient, OpenHABWidget widget)
        {
            await OnWidgetClickedAsync(widget);
        }

        private async Task TriggerItemCommand(object recipient, TriggerCommandMessage message)
        {
            HttpResponseResult<bool> result = await _openHabsdk.SendCommand(message.Item, message.Command).ConfigureAwait(false);
            if (!result.Content)
            {
                string errorMessage = AppResources.Errors.GetString("CommandFailed");
                errorMessage = string.Format(errorMessage, message.Command, message.Item?.Name);

                StrongReferenceMessenger.Default.Send<FireErrorMessage>(new FireErrorMessage(errorMessage));
            }
        }

        private void DataOperationState(object obj, DataOperation operation)
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

        #endregion

        /// <summary>
        /// Loads widgets for sitemap.
        /// </summary>
        /// <param name="version">OH version.</param>
        /// <returns>Task for async processing.</returns>
        public async Task LoadWidgets(OpenHABVersion version)
        {
            Widgets.Clear();

            ICollection<OpenHABWidget> widgetModels = await _openHabsdk.LoadItemsFromSitemap(Model, version).ConfigureAwait(false);
            widgetModels.ToList().ForEach(x => Widgets.Add(x));
        }

        public async Task LoadWidgets()
        {
            CurrentWidgets.Clear();
            StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

            await this.LoadWidgets(_serverInfo.Version).ConfigureAwait(false);

            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                SetWidgetsOnScreen(this.Widgets);
                StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));
            });
        }

        public async Task ReloadSitemap()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                CurrentWidgets?.Clear();
                StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

                if (SelectedWidget != null)
                {
                    await LoadWidgets().ConfigureAwait(false);
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
                    await LoadWidgets().ConfigureAwait(false);
                }

                StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));
                ReloadSitemapCommand.InvokeCanExecuteChanged(null);
            });
        }

        /// <summary>
        /// Navigate backwards between linked pages.
        /// </summary>
        public void WidgetGoBack(OpenHABWidget widget)
        {
            OpenHABWidget lastWidget = null;
            while (lastWidget == null || lastWidget.WidgetId != widget.WidgetId)
            {
                lastWidget = WidgetNavigationService.GoBack();
            }

            SelectedWidget = widget;

            SetWidgetsOnScreen(widget.LinkedPage.Widgets);
            StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget, widget));
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
                StrongReferenceMessenger.Default.Send<WigetNavigation>(new WigetNavigation(lastWidget, widget));

                SetWidgetsOnScreen(SelectedWidget?.LinkedPage?.Widgets);
            });
        }

        public async Task SelectWidget()
        {
            if (SelectedWidget != null)
            {
                await LoadWidgets().ConfigureAwait(false);
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
                await LoadWidgets().ConfigureAwait(false);
            }
        }

        public async void SetWidgetsOnScreen(ICollection<OpenHABWidget> widgets)
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                CurrentWidgets.Clear();
                CurrentWidgets.AddRange(widgets);
            });
        }
    }
}
