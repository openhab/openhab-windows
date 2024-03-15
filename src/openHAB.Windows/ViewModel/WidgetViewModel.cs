using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using Windows.UI;

namespace openHAB.Windows.ViewModel
{
    /// <summary>
    /// Represents the view model for a widget in the openHAB application.
    /// </summary>
    public class WidgetViewModel : ViewModelBase<OpenHABWidget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying model for the widget.</param>
        public WidgetViewModel(OpenHABWidget model)
            : base(model)
        {
            List<WidgetViewModel> children = new List<WidgetViewModel>();
            Model.Children.ToList().ForEach(w => children.Add(new WidgetViewModel(w)));
            Children = children;

            LinkedPage = Model.LinkedPage != null ? new SitemapViewModel(Model.LinkedPage) : null;
            IconPath = "C:\\Users\\ChristophHofmann\\AppData\\Local\\openHAB\\icons\\radiator.png"; //CacheAndRetriveLocalIconPath(Model.Icon).Result;
        }

        #region Properties

        /// <summary>
        /// Gets the collection of child widgets.
        /// </summary>
        public ICollection<WidgetViewModel> Children
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the icon of the widget.
        /// </summary>
        public string Icon
        {
            get => Model.Icon;
        }

        /// <summary>
        /// Gets the path of the icon.
        /// </summary>
        public string IconPath
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the item associated with the widget.
        /// </summary>
        public OpenHABItem Item
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
        public SitemapViewModel LinkedPage
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the collection of mappings for the widget.
        /// </summary>
        public ICollection<OpenHABWidgetMapping> Mappings
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
            get => Model.Refresh;
        }

        /// <summary>
        /// Gets the step value for the widget.
        /// </summary>
        public float Step
        {
            get => Model.Step;
        }

        public string State
        {
            get => Model.State;
            set
            {
                if (value.CompareTo(Model.State) == 0)
                {
                    return;
                }

                Model.State = value;
                OnPropertyChanged(nameof(State));
            }
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
        /// Gets or sets the value of the widget.
        /// </summary>
        public string Value
        {
            get => Model.Value;
            set
            {
                if (value.CompareTo(Model.Value) == 0)
                {
                    return;
                }

                Model.Value = value;
                OnPropertyChanged(nameof(Value));
            }
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
        /// Gets the ID of the widget.
        /// </summary>
        public string WidgetId
        {
            get => Model.WidgetId;
        }

        /// <summary>
        /// Gets the visibility of the widget.
        /// </summary>
        public Visibility Visibility
        {
            get => Model.Visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Encoding
        {
            get => Model.Encoding;
        }

        #endregion

        private async Task<string> CacheAndRetriveLocalIconPath(string icon)
        {
            IIconCaching iconCaching = DIService.Instance.GetService<IIconCaching>();
            ISettingsService settingsService = DIService.Instance.GetService<ISettingsService>();
            Settings setting = settingsService.Load();

            string iconFormat = setting.UseSVGIcons ? "svg" : "png";
            string path = await iconCaching.ResolveIconPath(icon, Model.State, iconFormat).ConfigureAwait(false);

            return path;
        }

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
