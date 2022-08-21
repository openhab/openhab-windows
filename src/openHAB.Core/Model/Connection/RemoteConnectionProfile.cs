using System;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;

namespace OpenHAB.Core.Model.Connection
{
    /// <summary>Connection profile for for local custom connection to OpenHab server.</summary>
    /// <seealso cref="OpenHAB.Core.Contracts.IConnectionProfile" />
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
