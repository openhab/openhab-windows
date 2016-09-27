using OpenHAB.Core.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OpenHAB.Windows.Converters
{
    public class WidgetTemplateSelector : DataTemplateSelector
    {
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

        public DataTemplate FrameTemplate { get; set; }

        public DataTemplate SwitchTemplate { get; set; }

        public DataTemplate SliderTemplate { get; set; }

        public DataTemplate TextTemplate { get; set; }

        public DataTemplate ImageTemplate { get; set; }
    }
}
