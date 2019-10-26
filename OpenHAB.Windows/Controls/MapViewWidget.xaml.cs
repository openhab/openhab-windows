using System;
using System.Globalization;
using System.Linq;
using OpenHAB.Windows.Extensions;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Widget control that represents an OpenHAB Map.
    /// </summary>
    public sealed partial class MapViewWidget : WidgetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewWidget"/> class.
        /// </summary>
        public MapViewWidget()
        {
            InitializeComponent();
            PopupDialog.AdjustSize();

#if RELEASE
            MapView.MapServiceToken = Don't forget to set the Bing Maps keys!;
            MapViewFull.MapServiceToken = Don't forget to set the Bing Maps keys!;
#endif
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetState();
        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            await PopupDialog.ShowAsync();
        }

        internal override void SetState()
        {
            if (!string.IsNullOrEmpty(Widget.Item.State))
            {
                var latLong = Widget.Item.State.Split(',');
                if (latLong?.Length == 2)
                {
                    double latitude = double.Parse(latLong[0], CultureInfo.InvariantCulture);
                    double longitude = double.Parse(latLong[1], CultureInfo.InvariantCulture);

                    MapView.Center = MapViewFull.Center = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude });

                    MapIcon mapIcon = new MapIcon();
                    mapIcon.Location = MapView.Center;
                    mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    mapIcon.ZIndex = 0;

                    MapView.MapElements.Add(mapIcon);
                    MapViewFull.MapElements.Add(mapIcon);
                }
            }
        }
    }
}
