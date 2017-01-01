using System;
using System.Net.Http;

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
        /// Forces creation of a new client, for example when the settings in the app are updated
        /// </summary>
        public static void ResetClient()
        {
            _client = null;
        }

        private static HttpClient InitClient()
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
            {
                return null;
            }

            return new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }
    }
}
