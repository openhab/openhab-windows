using System;
using System.Net;
using System.Net.Http;
using CommonServiceLocator;
using OpenHAB.Core.Contracts.Services;

namespace OpenHAB.Core.Common
{
    /// <summary>
    /// A sealed class that holds the instance of HttpClient for this lifetimescope.
    /// </summary>
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;

        /// <summary>
        /// Gets or sets the connection URL.
        /// </summary>
        public static string BaseUrl { get; set; }

        /// <summary>
        /// Fetch the HttpClient instance.
        /// </summary>
        /// <returns>The HttpClient instance</returns>
        public static HttpClient Client(OpenHABHttpClientType connectionType)
        {
            return _client ?? (_client = InitClient(connectionType));
        }

        /// <summary>
        /// Create an HttpClient instance for one-time use.
        /// </summary>
        /// <returns>The HttpClient instance.</returns>
        public static HttpClient DisposableClient(OpenHABHttpClientType connectionType)
        {
            return InitClient(connectionType, true);
        }

        /// <summary>
        /// Forces creation of a new client, for example when the settings in the app are updated.
        /// </summary>
        public static void ResetClient()
        {
            _client = null;
        }

        private static HttpClient InitClient(OpenHABHttpClientType connectionType, bool disposable = false)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl) && !disposable)
            {
                return null;
            }

            var handler = new HttpClientHandler();
            var credentials = GetCredentials(connectionType);

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

        private static NetworkCredential GetCredentials(OpenHABHttpClientType connectionType)
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().Load();
            string username = connectionType == OpenHABHttpClientType.Local ? settings.Username : settings.RemoteUsername;
            string password = connectionType == OpenHABHttpClientType.Local ? settings.Password : settings.RemotePassword;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return new NetworkCredential(username, password);
            }

            return null;
        }
    }
}
