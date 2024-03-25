using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Services;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.Converters
{
    /// <summary>
    /// Converts an icon path to a bitmap object.
    /// </summary>
    public class IconToBitmapConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ISettingsService settingsService = (ISettingsService)DIService.Instance.GetService<ISettingsService>();
            Settings settings = settingsService.Load();

            WidgetViewModel widget = value as WidgetViewModel;
            if (widget == null || string.IsNullOrEmpty(widget?.IconPath))
            {
                return null;
            }

            if (settings.UseSVGIcons)
            {
                return new SvgImageSource(new Uri(widget.IconPath));
            }
            else
            {
                return new BitmapImage(new Uri(widget.IconPath));
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
