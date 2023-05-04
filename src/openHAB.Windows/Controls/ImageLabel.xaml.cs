using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using System;
using System.Text.RegularExpressions;

namespace openHAB.Windows.Controls
{
    /// <summary>
    /// Control that represents an image with a label.
    /// </summary>
    public sealed partial class ImageLabel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageLabel"/> class.
        /// </summary>
        public ImageLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Bindable property for the control icon.
        /// </summary>
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
            "IconPath", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), IconChangedCallback));

#pragma warning disable S3168 // "async" methods should not return "void"
        private static async void IconChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
#pragma warning restore S3168 // "async" methods should not return "void"
        {
            var control = (ImageLabel)dependencyObject;
            if (control == null)
            {
                return;
            }

            // fix IconPathState by removing empty space and special characters
            string iconPath = control.IconPath;

            Match format = Regex.Match(iconPath, @"format=svg");
            Match state = Regex.Match(iconPath, @"state=(.+?)&");
            if (state != null)
            {
                if (!string.IsNullOrEmpty(state.Value))
                {
                    string newstate = Regex.Replace(state.Groups[1].Value, "[^0-9a-zA-Z.&]", string.Empty);
                    iconPath = control.IconPath.Replace(state.Groups[1].Value, newstate, StringComparison.InvariantCulture);
                }
            }

            IIconCaching iconCaching = DIService.Instance.GetService<IIconCaching>();
            iconPath = await iconCaching.ResolveIconPath(iconPath, format.Success ? "svg" : "png");

            if (format.Success)
            {
                control.Icon.Source = new SvgImageSource(new Uri(iconPath));
            }
            else
            {
                control.Icon.Source = new BitmapImage(new Uri(iconPath));
            }
        }

        /// <summary>
        /// Gets or sets the IconPath.
        /// </summary>
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value ?? string.Empty);
        }

        /// <summary>
        /// Bindable property for the label.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), TextChangedCallback));

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string LabelText
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value ?? string.Empty);
        }

        private static void TextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;

            if (control == null)
            {
                return;
            }

            control.Label.Text = control.LabelText;
        }

        /// <summary>
        /// The color for label text property.
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register(
         nameof(LabelForeground), typeof(SolidColorBrush), typeof(WidgetBase), new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Gets or sets the OpenHAB widget.
        /// </summary>
        public SolidColorBrush LabelForeground
        {
            get => (SolidColorBrush)GetValue(LabelForegroundProperty);
            set => SetValue(LabelForegroundProperty, value);
        }
    }
}
