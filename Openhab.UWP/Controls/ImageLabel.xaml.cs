using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Openhab.UWP.Controls
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
