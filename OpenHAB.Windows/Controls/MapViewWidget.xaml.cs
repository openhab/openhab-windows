using System;
using Microsoft.Practices.ServiceLocation;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using Windows.UI.Xaml;
using Windows.Devices.Geolocation;
using System.Linq;
using System.Globalization;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Foundation;

namespace OpenHAB.Windows.Controls
{
    public sealed partial class MapViewWidget : WidgetBase
    {
        public MapViewWidget()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            //_settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

            //TODO: Glenn add MapService Token
            //MapView.MapServiceToken = "";
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetState();
        }

        internal override void SetState()
        {
            if(!string.IsNullOrEmpty(Widget.Item.State))
            {
                var latLong = Widget.Item.State.Split(',');
                if (latLong?.Count() == 2)
                {
                    double latitude = double.Parse(latLong[0], CultureInfo.InvariantCulture);
                    double longitude = double.Parse(latLong[1], CultureInfo.InvariantCulture);

                    MapView.Center = new Geopoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude });

                    MapIcon mapIcon = new MapIcon();
                    mapIcon.Location = MapView.Center;
                    mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                    mapIcon.ZIndex = 0;

                    MapView.MapElements.Add(mapIcon);
                }
            }
        }
    }
}
