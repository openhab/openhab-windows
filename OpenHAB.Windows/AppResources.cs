using Windows.ApplicationModel.Resources;

namespace OpenHAB.Windows
{
    /// <summary>
    ///   Resources.
    /// </summary>
    public class AppResources
    {
        private static ResourceLoader _resourceLoader;

        /// <summary>
        ///   Gets the localized languages.
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
    }
}