using System;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Now you can use Generics to have cleaner code when enum parsing!
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <example>
    /// CT.Organ organNewParse = Enum<CT.Organ>.Parse("LENS");
    /// </example>
    public static class Enum<T>
    {
        public static T Parse(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }

    /// <summary>
    /// All different Widget Types
    /// </summary>
    public enum WidgetTypeEnum
    {
        /// <summary>
        /// Group
        /// </summary>
        Group,

        /// <summary>
        /// Frame
        /// </summary>
        Frame,

        /// <summary>
        /// Switch
        /// </summary>
        Switch,

        /// <summary>
        /// SectionSwitch
        /// </summary>
        SectionSwitch,

        /// <summary>
        /// RollerShutter
        /// </summary>
        RollerShutter,

        /// <summary>
        /// Slider
        /// </summary>
        Slider,

        /// <summary>
        /// DateTime
        /// </summary>
        DateTime,

        /// <summary>
        /// Text
        /// </summary>
        Text,

        /// <summary>
        /// Image
        /// </summary>
        Image,

        /// <summary>
        /// Selection
        /// </summary>
        Selection,

        /// <summary>
        /// Setpoint
        /// </summary>
        Setpoint,

        /// <summary>
        /// Chart
        /// </summary>
        Chart,

        /// <summary>
        /// Video
        /// </summary>
        Video,

        /// <summary>
        /// VideoMjpeg
        /// </summary>
        VideoMjpeg,

        /// <summary>
        /// Webview
        /// </summary>
        Webview,

        /// <summary>
        /// Colorpicker
        /// </summary>
        Colorpicker,

        /// <summary>
        /// Mapview
        /// </summary>
        Mapview,

        /// <summary>
        /// Generic
        /// </summary>
        Generic
    }
}
