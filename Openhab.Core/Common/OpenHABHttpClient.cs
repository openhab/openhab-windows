using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using OpenHAB.Core.Model;
using OpenHAB.Core.Services;

namespace OpenHAB.Core.Common
{
    /// <summary>
    /// A sealed class that holds the instance of HttpClient for this lifetimescope.
    /// </summary>
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;
        private static Settings _settings;
        private static ILogger<OpenHABHttpClient> _logger;
        private OpenHABConnection _connection;

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
        public HttpClient Client(OpenHABConnection connection, Settings settings)
        {
            _settings = settings;

            return _client ?? (_client = InitClient(connection));
        }

        /// <summary>
        /// Create an HttpClient instance for one-time use.
        /// </summary>
        /// <returns>The HttpClient instance.</returns>
        public HttpClient DisposableClient(OpenHABConnection connection, Settings settings)
        {
            _settings = settings;
            return InitClient(connection, true);
        }

        /// <summary>
        /// Forces creation of a new client, for example when the settings in the app are updated.
        /// </summary>
        public void ResetClient()
        {
            _client = null;
        }

        private HttpClient InitClient(OpenHABConnection connection, bool disposable = false)
        {
            _logger.LogInformation($"Initialize http client for connection type '{connection.Type.ToString()}'");

            if (string.IsNullOrWhiteSpace(BaseUrl) && !disposable)
            {
                return null;
            }

            var handler = new HttpClientHandler();

            if (connection.WillIgnoreSSLCertificate.HasValue && connection.WillIgnoreSSLHostname.HasValue)
            {
                _connection = connection;
                handler.ServerCertificateCustomValidationCallback = CheckValidationResult;
            }

            var credentials = GetCredentials(connection.Type);

            if (credentials != null)
            {
                handler.Credentials = credentials;
            }

            var client = new HttpClient(handler);
            if (!disposable)
            {
                client.BaseAddress = new Uri(BaseUrl);
            }

            return client;
        }

        private NetworkCredential GetCredentials(OpenHABHttpClientType connectionType)
        {
            string username = connectionType == OpenHABHttpClientType.Local ? _settings.LocalConnection.Username : _settings.RemoteConnection.Username;
            string password = connectionType == OpenHABHttpClientType.Local ? _settings.LocalConnection.Password : _settings.RemoteConnection.Password;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return new NetworkCredential(username, password);
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
