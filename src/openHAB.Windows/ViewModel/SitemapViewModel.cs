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
using openHAB.Windows.Messages;
using openHAB.Windows.Services;

namespace openHAB.Windows.ViewModel;

/// <summary>
/// ViewModel class for the sitemap view.
/// </summary>
public class SitemapViewModel : ViewModelBase<Sitemap>, IDisposable
{
    private readonly SitemapService _sitemapService;

    private ObservableCollection<WidgetViewModel> _currentWidgets;
    private readonly IServiceProvider _serviceProvider;
    private WidgetViewModel _selectedWidget;
    private ObservableCollection<WidgetViewModel> _widgets;
    private bool disposedValue;

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
    /// </summary>
    private SitemapViewModel()
        : base(new Sitemap())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitemapViewModel"/> class.
    /// </summary>
    /// <param name="model">Model class for view model.</param>
    /// <param name="serverInfo">openHAB Instance information.</param>
    private SitemapViewModel(Sitemap model, List<WidgetViewModel> widgetViewModels, SitemapService sitemapService, IServiceProvider serviceProvider)
         : base(model)
    {
        _widgets = new ObservableCollection<WidgetViewModel>(widgetViewModels ?? new List<WidgetViewModel>());
        _sitemapService = sitemapService;
        _currentWidgets = new ObservableCollection<WidgetViewModel>();
        _serviceProvider = serviceProvider;

        StrongReferenceMessenger.Default.Register<WidgetClickedMessage>(this, async (recipient, msg)
            =>
        {
            if (msg.Widget == null)
            {
                return;
            }

            WidgetViewModel viewModel = msg.Widget;
            await OnWidgetClickedAsync(viewModel);
        });

        StrongReferenceMessenger.Default.Register<TriggerCommandMessage>(this, async (recipient, msg)
            => await TriggerItemCommand(msg).ConfigureAwait(false));

        StrongReferenceMessenger.Default.Register<DataOperation>(this, (obj, operation)
            => DataOperationState(operation));

        StrongReferenceMessenger.Default.Register<WidgetNavigationMessage, string>(this, Model.Name, (recipient, msg) =>
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

        SetWidgetsOnScreenAsync(Widgets);
    }

    #endregion Constructors

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

    #endregion Properties

    #region Reload Command

    private ActionCommand _reloadSitemapCommand;

    /// <summary>
    /// Gets the command to reload sitemap and widget data.
    /// </summary>
    /// <value>
    /// The refresh command.
    /// </value>
    public ActionCommand ReloadSitemapCommand => _reloadSitemapCommand ?? (_reloadSitemapCommand = new ActionCommand(ExecuteReloadSitemapCommand, CanExecuteReloadSitemapCommand));

    private bool CanExecuteReloadSitemapCommand(object arg)
    {
        return !_canExecuteReloadSitemap;
    }

    private async void ExecuteReloadSitemapCommand(object obj)
    {
        await ReloadSitemap().ConfigureAwait(false);
    }

    private async Task ReloadSitemap()
    {
        CurrentWidgets?.Clear();
        StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Started));

        await LoadWidgetsAsync().ConfigureAwait(false);

        if (SelectedWidget != null)
        {
            WidgetViewModel widget = await FindWidgetAsync(SelectedWidget.WidgetId, Widgets).ConfigureAwait(false);
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

        StrongReferenceMessenger.Default.Send<DataOperation>(new DataOperation(OperationState.Completed));
        ReloadSitemapCommand.InvokeCanExecuteChanged(null);
    }

    #endregion Reload Command

    #region Navigate To Sitemap Root Command

    private bool _canExecuteReloadSitemap;
    private ActionCommand _navigateToSitemapRootCommand;

    /// <summary>
    /// Gets the command to navigate to the root of the sitemap.
    /// </summary>
    public ActionCommand NavigateToSitemapRoot =>
        _navigateToSitemapRootCommand ?? (_navigateToSitemapRootCommand = new ActionCommand(ExecuteNavigateToSitemapRootCommand, CanExecuteNavigateToSitemapRootCommand));

    private bool CanExecuteNavigateToSitemapRootCommand(object arg)
    {
        return true;
    }

    private void ExecuteNavigateToSitemapRootCommand(object obj)
    {
        WidgetNavigationService.ClearWidgetNavigation();
        SetWidgetsOnScreenAsync(Widgets);
        SelectedWidget = null;

        StrongReferenceMessenger.Default.Send(new WidgetNavigationMessage(SelectedWidget, null, EventTriggerSource.Widget), Model.Name);
    }

    private static async Task<List<WidgetViewModel>> ConvertWidgetsAsync(ICollection<Widget> widgets, IServiceProvider serviceProvider)
    {
        List<WidgetViewModel> widgetViewModels = new List<WidgetViewModel>();
        if (widgets == null)
        {
            return widgetViewModels;
        }

        foreach (Widget widget in widgets)
        {
            widgetViewModels.Add(await WidgetViewModel.CreateAsync(widget, serviceProvider).ConfigureAwait(false));
        }

        return widgetViewModels;
    }

    #endregion Navigate To Sitemap Root Command

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

    #endregion Events

    #region Factory

    /// <summary>
    /// Creates a new instance of the <see cref="SitemapViewModel"/> class asynchronously.
    /// </summary>
    /// <param name="sitemap">The OpenHABSitemap object.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created SitemapViewModel.</returns>
    public static async Task<SitemapViewModel> CreateAsync(Sitemap sitemap, SitemapService sitemapService, IServiceProvider serviceProvider)
    {
        List<WidgetViewModel> widgetViewModels = await ConvertWidgetsAsync(sitemap.Homepage?.Widgets, serviceProvider).ConfigureAwait(false);

        return new SitemapViewModel(sitemap, widgetViewModels, sitemapService, serviceProvider);
    }

    private async Task LoadWidgetsAsync()
    {
        CurrentWidgets?.Clear();

        ICollection<Widget> widgetModels = await _sitemapService.LoadItemsFromSitemapAsync(Model).ConfigureAwait(false);
        Widgets = new ObservableCollection<WidgetViewModel>(await ConvertWidgetsAsync(widgetModels, _serviceProvider).ConfigureAwait(false));

        await SetWidgetsOnScreenAsync(this.Widgets);
    }

    #endregion Factory

    #region Widget interaction

    private async Task<WidgetViewModel> FindWidgetAsync(string widgetId, ICollection<WidgetViewModel> widgets)
    {
        if (widgets == null || widgets.Count == 0)
        {
            return null;
        }

        foreach (WidgetViewModel widget in widgets)
        {
            if (string.CompareOrdinal(widget.WidgetId, widgetId) == 0)
            {
                return widget;
            }

            ICollection<WidgetViewModel> childWidgets = string.CompareOrdinal(widget.Type, "Group") == 0
                ? await ConvertWidgetsAsync(widget.LinkedPage?.Widgets, _serviceProvider).ConfigureAwait(false)
                : widget.Children;

            WidgetViewModel match = await FindWidgetAsync(widgetId, childWidgets).ConfigureAwait(false);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private async Task OnWidgetClickedAsync(WidgetViewModel widget)
    {
        await App.DispatcherQueue.EnqueueAsync(async () =>
        {
            WidgetViewModel lastWidget = SelectedWidget;
            SelectedWidget = widget;
            if (SelectedWidget.LinkedPage == null || !SelectedWidget.LinkedPage.Widgets.Any())
            {
                return;
            }

            WidgetNavigationService.Navigate(SelectedWidget);
            StrongReferenceMessenger.Default.Send(new WidgetNavigationMessage(lastWidget, widget, EventTriggerSource.Widget), Model.Name);

            List<WidgetViewModel> widgets = await ConvertWidgetsAsync(SelectedWidget.LinkedPage.Widgets, _serviceProvider).ConfigureAwait(false);
            await SetWidgetsOnScreenAsync(widgets);
        });
    }

    private async Task SetWidgetsOnScreenAsync(ICollection<WidgetViewModel> widgets)
    {
        await App.DispatcherQueue.EnqueueAsync(() =>
        {
            CurrentWidgets?.Clear();
            CurrentWidgets?.AddRange(widgets);
        });
    }

    private async Task WidgetGoBack(WidgetViewModel widget)
    {
        if (widget == null || !WidgetNavigationService.CanGoBack)
        {
            return;
        }

        WidgetViewModel lastWidget = SelectedWidget;
        WidgetViewModel widgetFromStack = WidgetNavigationService.GoBack();

        while (widgetFromStack != null && widgetFromStack.WidgetId != widget.WidgetId)
        {
            widgetFromStack = WidgetNavigationService.GoBack();
        }

        if (widgetFromStack == null)
        {
            return;
        }

        SelectedWidget = widgetFromStack;
        WidgetNavigationService.Navigate(SelectedWidget);
        StrongReferenceMessenger.Default.Send(new WidgetNavigationMessage(lastWidget, SelectedWidget, EventTriggerSource.Widget), Model.Name);

        List<WidgetViewModel> widgets = await ConvertWidgetsAsync(SelectedWidget.LinkedPage.Widgets, _serviceProvider).ConfigureAwait(false);
        await SetWidgetsOnScreenAsync(widgets);
    }

    #endregion Widget interaction

    #region Dispose

    /// <inheritdoc/>
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
                StrongReferenceMessenger.Default.Unregister<WidgetNavigationMessage, string>(this, Model.Name);

                Widgets = null;
                CurrentWidgets = null;
                SelectedWidget = null;
            }

            disposedValue = true;
        }
    }

    #endregion Dispose
}
