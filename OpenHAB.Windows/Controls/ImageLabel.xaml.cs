using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Controls
{
    public sealed partial class ImageLabel : UserControl
    {
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register(
            "IconPath", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), IconChangedCallback));

        private static void IconChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as ImageLabel;
            control.Icon.Source = new BitmapImage(new Uri(control.IconPath));
        }

        public string IconPath
        {
            get { return (string) GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ImageLabel), new PropertyMetadata(default(string), TextChangedCallback));

        private static void TextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as ImageLabel;
            control.Label.Text = control.LabelText;
        }

        public string LabelText
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public ImageLabel()
        {
            InitializeComponent();
        }
    }
}
