using openHAB.Core.Connection.Contracts;

namespace openHAB.Core.Connection
{
    /// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
    /// <seealso cref="IConnectionProfile" />
    public class CloudConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public bool AllowHostUrlConfiguration
        {
            get => false;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLCertificate
        {
            get => false;
        }

        /// <inheritdoc/>
        public bool AllowIgnoreSSLHostname
        {
            get => false;
        }

        /// <inheritdoc/>
        public int Id
        {
            get => 4;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => AppResources.Values.GetString("RemoteDefaultConnection");
        }

        /// <inheritdoc/>
        public OpenHABHttpClientType Type
        {
            get => OpenHABHttpClientType.Remote;
        }

        /// <inheritdoc/>
        public OpenHABConnection CreateConnection()
        {
            return new OpenHABConnection()
            {
                Type = OpenHABHttpClientType.Remote,
                Url = "https://myopenhab.org/",
            };
        }
    }
}
