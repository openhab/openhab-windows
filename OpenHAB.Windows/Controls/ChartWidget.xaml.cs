using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB slider
    /// </summary>
    public sealed partial class ChartWidget : WidgetBase
    {
        public static readonly DependencyProperty ChartUriProperty = DependencyProperty.Register(
            "ChartUri", typeof(string), typeof(ChartWidget), new PropertyMetadata(default(string)));

        public string ChartUri
        {
            get { return (string) GetValue(ChartUriProperty); }
            set { SetValue(ChartUriProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartWidget"/> class.
        /// </summary>
        public ChartWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.Load();
            var serverUrl = settings.IsRunningInDemoMode.Value ? Constants.Api.DemoModeUrl : settings.OpenHABUrl;

            if (!serverUrl.EndsWith("/"))
            {
                serverUrl += "/";
            }

            // http://demo.openhab.org:8080/chart?groups=Weather_Chart&period=d
            ChartUri = $"{serverUrl}chart?groups={Widget.Item.Name}&period=d";
            Chart.Source = new BitmapImage(new Uri(ChartUri));
        }
    }
}
