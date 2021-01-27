using Windows.ApplicationModel.Resources;

namespace OpenHAB.Core
{
    /// <summary>
    ///   Resources.
    /// </summary>
    public class AppResources
    {
        private static ResourceLoader _resourceLoader;

        /// <summary>
        ///   Gets the localized UI values.
        /// </summary>
        public static ResourceLoader Values
        {
            get
            {
                if (_resourceLoader == null)
                {
                    _resourceLoader = ResourceLoader.GetForViewIndependentUse("Resources");
                }

                return _resourceLoader;
            }
        }

        /// <summary>
        ///   Gets the localized error strings.
        /// </summary>
        public static ResourceLoader Errors
        {
            get
            {
                if (_resourceLoader == null)
                {
                    _resourceLoader = ResourceLoader.GetForViewIndependentUse("Errors");
                }

                return _resourceLoader;
            }
        }
    }
}