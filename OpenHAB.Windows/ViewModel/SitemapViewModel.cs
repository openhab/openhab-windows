using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Windows.Services;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// A class that represents an OpenHAB sitemap.
    /// </summary>
    public class SitemapViewModel : ViewModelBase<OpenHABSitemap>
    {
        private ObservableCollection<WidgetViewModel> _widgets;
        private IOpenHAB _openHabsdk;

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
        public SitemapViewModel(OpenHABSitemap model)
             : base(model)
        {
            _widgets = new ObservableCollection<WidgetViewModel>();
            _openHabsdk = (IOpenHAB)DIService.Instance.Services.GetService(typeof(IOpenHAB));

            //Model.Widgets.ToList().ForEach(x => _widgets.Add(new WidgetViewModel(x)));
        }

        /// <summary>
        /// Gets or sets the name of the OpenHAB sitemap.
        /// </summary>
        public string Name
        {
            get
            {
                return Model.Name;
            }
        }

        /// <summary>
        /// Gets or sets the label of the OpenHAB sitemap.
        /// </summary>
        public string Label
        {
            get
            {
                return Model.Label;
            }
        }

        ///// <summary>
        ///// Gets or sets the link of the OpenHAB sitemap.
        ///// </summary>
        //public string Link
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the icon of the OpenHAB sitemap.
        ///// </summary>
        //public string Icon
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the url of the OpenHAB sitemap.
        ///// </summary>
        //public string HomepageLink
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether the sitemap is a leaf.
        ///// </summary>
        //public bool Leaf
        //{
        //    get; set;
        //}

        /// <summary>
        /// Gets or sets the title of the sitemap.
        /// </summary>
        public string Title
        {
            get
            {
                return Model.Title;
            }
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

                _widgets = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadWidgets(OpenHABVersion version)
        {
            ICollection<OpenHABWidget> widgetModels = await _openHabsdk.LoadItemsFromSitemap(Model, version).ConfigureAwait(false);
            widgetModels.ToList().ForEach(x => _widgets.Add(new WidgetViewModel(x)));
        }
    }
}
