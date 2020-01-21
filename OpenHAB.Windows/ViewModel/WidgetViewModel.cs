using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Windows.Services;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// A class that represents an OpenHAB widget.
    /// </summary>
    public class WidgetViewModel : ViewModelBase<OpenHABWidget>
    {
        private string _icon;
        private string _label;
        private SitemapViewModel _linkedPage;
        private IOpenHAB _openHabsdk;
        private ObservableCollection<WidgetViewModel> _children;


        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        public WidgetViewModel()
            : base(new OpenHABWidget())
        {
            _children = new ObservableCollection<WidgetViewModel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetViewModel"/> class.
        /// </summary>
        /// <param name="model">Model class.</param>
        public WidgetViewModel(OpenHABWidget model)
            : base(model)
        {
            _children = new ObservableCollection<WidgetViewModel>();
            Model.Children.ToList().ForEach(x => _children.Add(new WidgetViewModel(x)));

            _linkedPage = new SitemapViewModel(Model.LinkedPage);
        }

        ///// <summary>
        ///// Gets or sets the ID of the OpenHAB widget.
        ///// </summary>
        //public string Id
        //{
        //    get; set;
        //}

        /// <summary>
        /// Gets or sets the Label of the OpenHAB widget.
        /// </summary>
        public string Label
        {
            get => Model.Label;
        }

        ///// <summary>
        ///// Gets or sets the Value of the widget.
        ///// </summary>
        //public string Value
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Icon of the OpenHAB widget.
        ///// </summary>
        //public string Icon
        //{
        //    get => _icon ?? string.Empty;
        //    set => _icon = value;
        //}

        ///// <summary>
        ///// Gets or sets the Type of the OpenHAB widget.
        ///// </summary>
        //public string Type
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Url of the OpenHAB widget.
        ///// </summary>
        //public string Url
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Period of the OpenHAB widget.
        ///// </summary>
        //public string Period
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Service of the OpenHAB widget.
        ///// </summary>
        //public string Service
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the MinValue of the OpenHAB widget.
        ///// </summary>
        //public float MinValue
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the MaxValue of the OpenHAB widget.
        ///// </summary>
        //public float MaxValue
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Step of the OpenHAB widget.
        ///// </summary>
        //public float Step
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Refresh of the OpenHAB widget.
        ///// </summary>
        //public int Refresh
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Height of the OpenHAB widget.
        ///// </summary>
        //public int Height
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the State of the OpenHAB widget.
        ///// </summary>
        //public string State
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the IconColor of the OpenHAB widget.
        ///// </summary>
        //public string IconColor
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the LabelColor of the OpenHAB widget.
        ///// </summary>
        //public string LabelColor
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the ValueColor of the OpenHAB widget.
        ///// </summary>
        //public string ValueColor
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Encoding of the OpenHAB widget.
        ///// </summary>
        //public string Encoding
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Item of the OpenHAB widget.
        ///// </summary>
        //public OpenHABItem Item
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Parent of the OpenHAB widget.
        ///// </summary>
        //public OpenHABWidget Parent
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets or sets the Children of the OpenHAB widget.
        ///// </summary>
        public ObservableCollection<WidgetViewModel> Children
        {
            get
            {
                return _children;
            }
        }

        ///// <summary>
        ///// Gets or sets the Mapping of the OpenHAB widget.
        ///// </summary>
        //public ICollection<OpenHABWidgetMapping> Mappings
        //{
        //    get; set;
        //}

        /// <summary>
        /// Gets or sets the linked page when available.
        /// </summary>
        public SitemapViewModel LinkedPage
        {
            get
            {
               return _linkedPage;
            }
        }
    }
}
