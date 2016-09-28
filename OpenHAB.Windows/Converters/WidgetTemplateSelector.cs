using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Converters
{
    /// <summary>
    /// TemplateSelector that determines what widget needs to be shown in the UI
    /// </summary>
    public class WidgetTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc/>
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var widget = item as OpenHABWidget;

            if (widget == null)
            {
                return FrameTemplate;
            }

            switch (widget.Type.ToLower())
            {
                case "frame":
                    return FrameTemplate;
                case "switch":
                    return SwitchTemplate;
                case "slider":
                    return SliderTemplate;
                case "text":
                case "numberitem":
                    return TextTemplate;
                case "image":
                    return ImageTemplate;
                default:
                    return FrameTemplate;
            }
        }

        /// <summary>
        /// Gets or sets the template for a Frame control
        /// </summary>
        public DataTemplate FrameTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Switch control
        /// </summary>
        public DataTemplate SwitchTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Slider control
        /// </summary>
        public DataTemplate SliderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Text control
        /// </summary>
        public DataTemplate TextTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a Image control
        /// </summary>
        public DataTemplate ImageTemplate { get; set; }
    }
}
