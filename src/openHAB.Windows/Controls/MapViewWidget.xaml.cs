using System;
using System.Globalization;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.WinUI;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using openHAB.Windows.Extensions;

namespace openHAB.Windows.Controls
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

            MapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapViewFull.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            MapViewFull.Map.Widgets.Add(new ScaleBarWidget(MapViewFull.Map));
            MapViewFull.Map.Widgets.Add(new ZoomInOutWidget());

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetState();
        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;

            PopupDialog.XamlRoot = this.XamlRoot;
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

                    var coordinate = SphericalMercator.FromLonLat(longitude, latitude).ToMPoint();

                    MapView.Map.Home = n => n.NavigateTo(coordinate, MapView.Map.Resolutions[19]);
                    MapViewFull.Map.Home = n => n.NavigateTo(coordinate, MapView.Map.Resolutions[13]);
                    
                    //TODO: Implement mapicon
                    //MapIcon mapIcon = new MapIcon();
                    //mapIcon.Location = MapView.Center;
                    //mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    //mapIcon.ZIndex = 0;

                    //MapView.MapElements.Add(mapIcon);
                    //MapViewFull.MapElements.Add(mapIcon);
                }
            }
        }
    }
}
