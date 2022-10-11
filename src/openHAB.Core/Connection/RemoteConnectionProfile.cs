using System;
using openHAB.Core.Connection.Contracts;
using openHAB.Core;
using openHAB.Core.Common;

namespace openHAB.Core.Connection
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
            };
        }
    }
}
