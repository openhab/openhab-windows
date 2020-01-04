using System.Collections.Generic;

namespace OpenHAB.Core.Model
{
    /// <summary>
    /// Class that holds all the OpenHAB Windows app settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            WillIgnoreSSLCertificate = false;
            WillIgnoreSSLHostname = false;
            IsRunningInDemoMode = false;

            LocalConnection = new OpenHABConnection();
            RemoteConnection = new OpenHABConnection();
        }

        /// <summary>
        /// Gets or sets the if the default sitemap should be hidden.
        /// </summary>
        /// <value>The hide default sitemap.</value>
        public bool? HideDefaultSitemap
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the app is currently running in demo mode.
        /// </summary>
        public bool? IsRunningInDemoMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the config to the OpenHAB remote instance.
        /// </summary>
        public OpenHABConnection LocalConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the config to the OpenHAB remote instance.
        /// </summary>
        public OpenHABConnection RemoteConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application language.
        /// </summary>
        /// <value>The application language.</value>
        public string AppLanguage
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL certificate.
        /// </summary>
        public bool? WillIgnoreSSLCertificate
        {
            get;
            set;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the app will ignore the SSL hostname.
        /// </summary>
        public bool? WillIgnoreSSLHostname
        {
            get;
            set;
        }
    }
}
