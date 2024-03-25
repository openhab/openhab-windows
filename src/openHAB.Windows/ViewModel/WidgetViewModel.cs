using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// Represents the view model for a widget in the openHAB application.
    /// </summary>
    public class WidgetViewModel : ViewModelBase<Widget>
    {
        private ObservableCollection<WidgetViewModel> _children;
        private string _iconPath;
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying model for the widget.</param>
        private WidgetViewModel(Widget model)
            : base(model)
        {
            Children = new ObservableCollection<WidgetViewModel>();
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
                Color color = ConvertColorCodeToColor(colorString);
                return new SolidColorBrush(color);
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
                Color color = ConvertColorCodeToColor(Model.ValueColor);
                return new SolidColorBrush(color);
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
        public static async Task<WidgetViewModel> CreateAsync(Widget model)
        {
            WidgetViewModel viewModel = new WidgetViewModel(model);
            viewModel.LoadData();

            return viewModel;
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
            foreach (Widget w in Model.Children)
            {
                WidgetViewModel viewModel = await WidgetViewModel.CreateAsync(w);
                widgets.Add(viewModel);
            }

            Children = widgets;
        }

        private async Task<string> CacheAndRetrieveLocalIconPath(string icon)
        {
            IIconCaching iconCaching = DIService.Instance.GetService<IIconCaching>();
            ISettingsService settingsService = DIService.Instance.GetService<ISettingsService>();
            Settings setting = settingsService.Load();

            string iconFormat = setting.UseSVGIcons ? "svg" : "svg";
            string path = await iconCaching.ResolveIconPath(icon, Model.State, iconFormat).ConfigureAwait(false);

            return path;
        }

        #endregion

        private Color ConvertColorCodeToColor(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return (Color)Application.Current.Resources["TextFillColorPrimary"];
            }

            return (Color)XamlBindingHelper.ConvertValue(typeof(Color), value);
        }
    }
}
