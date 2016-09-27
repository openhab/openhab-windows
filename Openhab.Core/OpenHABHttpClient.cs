using System;
using System.Net.Http;

namespace OpenHAB.Core
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
        public static string BaseUrl { get; set; } = "http://192.168.1.7:8080";

        /// <summary>
        /// Fetch the HttpClient instance
        /// </summary>
        /// <returns>The HttpClient instance</returns>
        public static HttpClient Client()
        {
            return _client ?? (_client = InitClient());
        }

        private static HttpClient InitClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }
    }
}
