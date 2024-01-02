using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using openHAB.Core.Client.Connection.Models;

namespace openHAB.Core.Client
{
    /// <summary>
    /// A sealed class that holds the instance of HttpClient for this lifetime scope.
    /// </summary>
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;
        private static ILogger<OpenHABHttpClient> _logger;
        private Connection.Models.Connection _connection;

        /// <summary>Initializes a new instance of the <see cref="OpenHABHttpClient"/> class.</summary>
        /// <param name="logger">The logger.</param>
        public OpenHABHttpClient(ILogger<OpenHABHttpClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the connection URL.
        /// </summary>
        public static string BaseUrl
        {
            get; set;
        }

        /// <summary>
        /// Fetch the HttpClient instance.
        /// </summary>
        /// <returns>The HttpClient instance.</returns>
        public HttpClient Client(Connection.Models.Connection connection)
        {
            return _client ?? (_client = InitClient(connection));
        }

        /// <summary>
        /// Create an HttpClient instance for one-time use.
        /// </summary>
        /// <returns>The HttpClient instance.</returns>
        public HttpClient DisposableClient(Connection.Models.Connection connection)
        {
            return InitClient(connection, true);
        }

        /// <summary>
        /// Forces creation of a new client, for example when the settings in the app are updated.
        /// </summary>
        public void ResetClient()
        {
            _client = null;
        }

        private HttpClient InitClient(Connection.Models.Connection connection, bool disposable = false)
        {
            _logger.LogInformation($"Initialize HTTP client for connection type '{connection?.Type.ToString()}'");

            if (string.IsNullOrWhiteSpace(BaseUrl) && !disposable)
            {
                return null;
            }

            HttpClientHandler handler = new HttpClientHandler();
            if (connection.WillIgnoreSSLCertificate.HasValue && connection.WillIgnoreSSLHostname.HasValue)
            {
                _connection = connection;
                handler.ServerCertificateCustomValidationCallback = CheckValidationResult;
            }

            NetworkCredential credentials = GetCredentials(connection);
            if (credentials != null)
            {
                handler.Credentials = credentials;
            }

            HttpClient client = new HttpClient(handler);
            if (!disposable)
            {
                client.BaseAddress = new Uri(BaseUrl);
            }

            return client;
        }

        private NetworkCredential GetCredentials(Connection.Models.Connection connection)
        {
            if (!string.IsNullOrWhiteSpace(connection.Username) && !string.IsNullOrWhiteSpace(connection.Password))
            {
                return new NetworkCredential(connection.Username, connection.Password);
            }

            return null;
        }

        private bool CheckValidationResult(HttpRequestMessage message, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = true;
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return result;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                result &= _connection.WillIgnoreSSLCertificate.Value;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                result &= _connection.WillIgnoreSSLHostname.Value;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                result = false;
            }

            return result;
        }
    }
}
