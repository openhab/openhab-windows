using System;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts;

namespace OpenHAB.Core.Model.Connection
{
    /// <summary>Connection profile for for local default connection to OpenHab server.</summary>
    /// <seealso cref="OpenHAB.Core.Contracts.IConnectionProfile" />
    public class DefaultConnectionProfile : IConnectionProfile
    {
        /// <inheritdoc/>
        public OpenHABHttpClientType Type
        {
            get => OpenHABHttpClientType.Local;
        }

        /// <inheritdoc/>
        public string Name
        {
            get => "Default Local Connection";
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
        public bool AllowHostUrlConfiguration
        {
            get => false;
        }

        /// <inheritdoc/>
        public OpenHABConnection CreateConnection()
        {
            return new OpenHABConnection()
            {
                Url = "https://openhab:8443",
                Type = OpenHABHttpClientType.Local,
                WillIgnoreSSLCertificate = true,
                WillIgnoreSSLHostname = true,
            };
        }
    }
}
