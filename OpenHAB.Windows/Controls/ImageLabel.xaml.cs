using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Control that represents an image with a label
    /// </summary>
    public sealed partial class ImageLabel : UserControl
    {
        /// <summary>
        /// Bindable property for the control icon
        /// </summary>
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
            "IconPath", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), IconChangedCallback));

        private static void IconChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = (ImageLabel)dependencyObject;
            if (control == null)
            {
                return;
            }

            control.Icon.Source = new BitmapImage(new Uri(control.IconPath));
        }

        /// <summary>
        /// Gets or sets the IconPath
        /// </summary>
        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value ?? string.Empty);
        }

        /// <summary>
        /// Bindable property for the label
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), TextChangedCallback));

        /// <summary>
        /// Gets or sets the label
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
        /// Initializes a new instance of the <see cref="ImageLabel"/> class.
        /// </summary>
        public ImageLabel()
        {
            InitializeComponent();
        }
    }
}
