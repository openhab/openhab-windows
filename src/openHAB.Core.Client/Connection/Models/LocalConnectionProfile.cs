using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models
{
    /// <summary>Connection profile for local custom connection to OpenHab server.</summary>
    /// <seealso cref="IConnectionProfile" />
    public class LocalConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public bool AllowHostUrlConfiguration
        {
            get => true;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLCertificate
        {
            get => true;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLHostname
        {
            get => true;
        }

        /// <summary>Gets the connection profile îd.</summary>
        /// <value>The îd.</value>
        public int Id
        {
            get => 2;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("LocalCustomConnection");
        }

        /// <inheritdoc/>
        public HttpClientType Type
        {
            get => HttpClientType.Local;
        }

        /// <inheritdoc/>
        public Connection CreateConnection()
        {
            return new Connection()
            {
                Type = HttpClientType.Local,
            };
        }
    }
}
