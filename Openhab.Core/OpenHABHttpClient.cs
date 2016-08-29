using System;
using System.Net.Http;

namespace Openhab.Core
{
    public sealed class OpenHABHttpClient
    {
        private static HttpClient _client;

        public static string BaseUrl { get; set; } = "http://192.168.1.7:8080";

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
