using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB slider
    /// </summary>
    public sealed partial class ChartWidget : WidgetBase
    {
        private readonly ISettingsService _settingsService;

        private DispatcherTimer _timer;
        private string _chartUri;

        /// <summary>
        /// Gets or sets the assembled URI for the chart
        /// </summary>
        public string ChartUri
        {
            get => _chartUri;
            set
            {
                _chartUri = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartWidget"/> class.
        /// </summary>
        public ChartWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            _settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetImage();

            SetSources();
            InitTimer();
        }

        private void SetImage()
        {
            var settings = _settingsService.Load();
            var serverUrl = settings.IsRunningInDemoMode.Value ? Constants.Api.DemoModeUrl : settings.OpenHABUrl;

            if (!serverUrl.EndsWith("/"))
            {
                serverUrl += "/";
            }

            // http://demo.openhab.org:8080/chart?groups=Weather_Chart&period=d
            ChartUri = $"{serverUrl}chart?groups={Widget.Item.Name}&period={Widget.Period}";
        }

        private void InitTimer()
        {
            if (Widget.Refresh <= 0)
            {
                return;
            }

            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, Widget.Refresh) };
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void TimerOnTick(object sender, object o)
        {
            SetImage();
            SetSources();
        }

        private void SetSources()
        {
            ThumbImage.Source = new BitmapImage(
                new Uri(ChartUri, UriKind.Absolute))
                { CreateOptions = BitmapCreateOptions.IgnoreImageCache };

            FullImage.Source = new BitmapImage(
                new Uri(ChartUri, UriKind.Absolute))
            { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
        }

        private async void ImageWidget_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await PopupDialog.ShowAsync();
        }
    }
}
