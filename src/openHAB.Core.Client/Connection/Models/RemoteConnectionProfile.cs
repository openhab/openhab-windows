using openHAB.Common;
using openHAB.Core.Client.Connection.Contracts;

namespace openHAB.Core.Client.Connection.Models
{
    /// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
    /// <seealso cref="IConnectionProfile" />
    public class RemoteConnectionProfile : IConnectionProfile
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

        /// <inheritdoc/>
        public int Id
        {
            get => 3;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("RemoteCustomConnection");
        }

        /// <inheritdoc/>
        public HttpClientType Type
        {
            get => HttpClientType.Remote;
        }

        /// <inheritdoc/>
        public Connection CreateConnection()
        {
            return new Connection()
            {
                Type = HttpClientType.Remote,
            };
        }
    }
}
