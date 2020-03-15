using System.Collections.Generic;
using OpenHAB.Core.Contracts;
using OpenHAB.Core.Model.Connection;

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
            IsRunningInDemoMode = false;

            LocalConnection = new OpenHABConnection();
            RemoteConnection = new OpenHABConnection();

            ConnectionProfiles = new List<IConnectionProfile>();
            ConnectionProfiles.Add(new LocalConnectionProfile());
            ConnectionProfiles.Add(new DefaultConnectionProfile());
            ConnectionProfiles.Add(new RemoteConnectionProfile());
            ConnectionProfiles.Add(new CloudConnectionProfile());
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

        /// <summary>Gets the list of available connection profiles.</summary>
        /// <value>The connection profiles.</value>
        public List<IConnectionProfile> ConnectionProfiles
        {
            get;
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
    }
}
