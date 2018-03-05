using System;
using System.Net;
using System.Net.Http;
using GalaSoft.MvvmLight.Ioc;
using OpenHAB.Core.Contracts.Services;


namespace OpenHAB.Core.Common
{
    /// <summary>
    /// A sealed class that holds the instance of HttpClient for this lifetimescope
    /// </summary>
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;

        /// <summary>
        /// Gets or sets the connection URL
        /// </summary>
        public static string BaseUrl { get; set; }

        /// <summary>
        /// Fetch the HttpClient instance
        /// </summary>
        /// <returns>The HttpClient instance</returns>
        public static HttpClient Client()
        {
            return _client ?? (_client = InitClient());
        }

        /// <summary>
        /// Create an HttpClient instance for one-time use
        /// </summary>
        /// <returns>The HttpClient instance</returns>
        public static HttpClient DisposableClient()
        {
            return InitClient(true);
        }

        /// <summary>
        /// Forces creation of a new client, for example when the settings in the app are updated
        /// </summary>
        public static void ResetClient()
        {
            _client = null;
        }

        private static HttpClient InitClient(bool disposable = false)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl) && !disposable)
            {
                return null;
            }

            var handler = new HttpClientHandler();
            var credentials = GetCredentials();

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

        private static NetworkCredential GetCredentials()
        {
            var settings = SimpleIoc.Default.GetInstance<ISettingsService>().Load();
            string username = settings.Username;
            string password = settings.Password;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                return new NetworkCredential(username, password);
            }

            return null;
        }
    }
}
