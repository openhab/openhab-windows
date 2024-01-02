using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models
{
    /// <summary>Connection profile for local default connection to OpenHab server.</summary>
    /// <seealso cref="IConnectionProfile" />
    public class DefaultConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public bool AllowHostUrlConfiguration
        {
            get => false;
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

        /// <inheritdoc/>
        public int Id
        {
            get => 1;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("LocalDefaultConnection");
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
                Url = "https://openhab:8443",
                Type = HttpClientType.Local,
                WillIgnoreSSLCertificate = true,
                WillIgnoreSSLHostname = true,
            };
        }
    }
}
