using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Client.Models;
using openHAB.Core.Services.Contracts;
using Windows.UI;

namespace openHAB.Windows.ViewModel;

/// <summary>
/// Represents the view model for a widget in the openHAB application.
/// </summary>
public class WidgetViewModel : ViewModelBase<Widget>
{
    private readonly IServiceProvider _serviceProvider;
    private ObservableCollection<WidgetViewModel> _children;
    private string _iconPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
    /// </summary>
    /// <param name="model">The underlying model for the widget.</param>
    private WidgetViewModel(Widget model, IServiceProvider serviceProvider)
        : base(model)
    {
        Children = new ObservableCollection<WidgetViewModel>();
        _serviceProvider = serviceProvider;
    }

    #region Properties

    /// <summary>
    /// Gets the collection of child widgets.
    /// </summary>
    public ObservableCollection<WidgetViewModel> Children
    {
        get => _children;
        set => Set(ref _children, value);
    }

    /// <summary>
    /// Gets the encoding of the widget.
    /// </summary>
    public string Encoding
    {
        get => Model.Encoding;
    }

    /// <summary>
    /// Gets the icon of the widget.
    /// </summary>
    public SvgImageSource Icon
    {
        get => !string.IsNullOrEmpty(IconPath) ? new SvgImageSource(new Uri(IconPath)) : null;
    }

    /// <summary>
    /// Gets the path of the icon.
    /// </summary>
    public string IconPath
    {
        get => _iconPath;
        set => Set(ref _iconPath, value);
    }

    /// <summary>
    /// Gets the item associated with the widget.
    /// </summary>
    public Item Item
    {
        get => Model.Item;
    }

    /// <summary>
    /// Gets the label of the widget.
    /// </summary>
    public string Label
    {
        get => Model.Label;
    }

    /// <summary>
    /// Gets the color of the label.
    /// </summary>
    public SolidColorBrush LabelColor
    {
        get
        {
            string colorString = Model.LabelColor as string;
            return ColorValueToSolidColor(colorString);
        }
    }

    /// <summary>
    /// Gets the linked page of the widget.
    /// </summary>
    public Page LinkedPage
    {
        get; internal set;
    }

    /// <summary>
    /// Gets the collection of mappings for the widget.
    /// </summary>
    public ICollection<WidgetMapping> Mappings
    {
        get => Model.Mappings;
    }

    /// <summary>
    /// Gets the maximum value for the widget.
    /// </summary>
    public float MaxValue
    {
        get => Model.MaxValue;
    }

    /// <summary>
    /// Gets the minimum value for the widget.
    /// </summary>
    public float MinValue
    {
        get => Model.MinValue;
    }

    /// <summary>
    /// Gets or sets the parent widget view model.
    /// </summary>
    public WidgetViewModel Parent
    {
        get;
        internal set;
    }

    /// <summary>
    /// Gets the period for the widget.
    /// </summary>
    public string Period
    {
        get => Model.Period;
    }

    /// <summary>
    /// Gets the chart line interpolation (e.g. "linear", "step").
    /// </summary>
    public string Interpolation
    {
        get => Model.Interpolation;
    }

    /// <summary>
    /// Gets a value indicating whether the command is sent only on release.
    /// </summary>
    public bool ReleaseOnly
    {
        get => Model.ReleaseOnly;
    }

    /// <summary>
    /// Gets a value indicating whether the widget is momentary and does not reflect item state.
    /// </summary>
    public bool Stateless
    {
        get => Model.Stateless;
    }

    /// <summary>
    /// Gets the refresh rate for the widget.
    /// </summary>
    public int Refresh
    {
        get => (int)Model.Refresh;
    }

    /// <summary>
    /// Gets or sets the state of the widget.
    /// </summary>
    public string State
    {
        get => Model.Item != null && string.Compare(Model.Item?.State, "null", true) != 0 ? Model.Item.State : string.Empty;
        set
        {
            if (value.CompareTo(Model.Item?.State) == 0)
            {
                return;
            }

            Model.Item.State = value;
            OnPropertyChanged(nameof(State));
        }
    }

    /// <summary>
    /// Gets the step value for the widget.
    /// </summary>
    public float Step
    {
        get => Model.Step;
    }

    /// <summary>
    /// Gets the type of the widget.
    /// </summary>
    public string Type
    {
        get => Model.Type;
    }

    /// <summary>
    /// Gets the URL associated with the widget.
    /// </summary>
    public string Url
    {
        get => Model.Url;
    }

    /// <summary>
    /// Gets the color of the value.
    /// </summary>
    public SolidColorBrush ValueColor
    {
        get
        {
            string colorString = Model.ValueColor as string;
            return ColorValueToSolidColor(colorString);
        }
    }

    /// <summary>
    /// Gets the visibility of the widget.
    /// </summary>
    public Visibility Visibility
    {
        get => Model.Visibility ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Gets the ID of the widget.
    /// </summary>
    public string WidgetId
    {
        get => Model.WidgetId;
    }

    #endregion

    #region Factory

    /// <summary>
    /// Creates a new instance of the <see cref="WidgetViewModel"/> class asynchronously.
    /// </summary>
    /// <param name="model">The underlying model for the widget.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="WidgetViewModel"/>.</returns>
    public static async Task<WidgetViewModel> CreateAsync(Widget model, IServiceProvider serviceProvider)
    {
        WidgetViewModel viewModel = new WidgetViewModel(model, serviceProvider);
        viewModel.LoadData();

        return viewModel;
    }

    private async Task<string> CacheAndRetrieveLocalIconPath(string icon)
    {
        IIconCaching iconCaching = _serviceProvider.GetRequiredService<IIconCaching>();
        string path = await iconCaching.ResolveIconPath(icon, Model.State, "svg").ConfigureAwait(false);

        return path;
    }

    /// <summary>
    /// Loads the data for the widget asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadData()
    {
        if (!string.IsNullOrEmpty(Model.Icon))
        {
            IconPath = await CacheAndRetrieveLocalIconPath(Model.Icon);
        }

        if (Model.LinkedPage != null)
        {
            LinkedPage = Model.LinkedPage;
        }

        ObservableCollection<WidgetViewModel> widgets = new ObservableCollection<WidgetViewModel>();
        foreach (Widget widget in Model.Children)
        {
            WidgetViewModel viewModel = await WidgetViewModel.CreateAsync(widget, _serviceProvider);
            widgets.Add(viewModel);
        }

        Children = widgets;
    }

    #endregion

    #region Color Handling

    private SolidColorBrush ColorValueToSolidColor(string colorString, string resourceName = "TextFillColorPrimaryBrush")
    {
        if (string.IsNullOrEmpty(colorString))
        {
            ResourceDictionary? resourceDictionary = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(rd => rd.ContainsKey(resourceName));

            if (resourceDictionary == null || !resourceDictionary.TryGetValue(resourceName, out object defaultTextColor))
            {
                return null;
            }

            return (SolidColorBrush)defaultTextColor;
        }

        Color color = ConvertColorCodeToColor(colorString);
        return new SolidColorBrush(color);
    }

    private Color ConvertColorCodeToColor(string value)
    {
        return (Color)XamlBindingHelper.ConvertValue(typeof(Color), value);
    }

    #endregion
}
