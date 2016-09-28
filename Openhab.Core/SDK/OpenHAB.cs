using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.SDK
{
    /// <summary>
    /// The main SDK implementation of the connection to OpenHAB
    /// </summary>
    public class OpenHAB : IOpenHAB
    {
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHAB"/> class.
        /// </summary>
        /// <param name="settingsService">The service to fetch the settings</param>
        public OpenHAB(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            var settings = _settingsService.Load();
            OpenHABHttpClient.BaseUrl = settings.OpenHABUrl;
        }

        /// <inheritdoc />
        public void ResetConnection()
        {
            var settings = _settingsService.Load();
            OpenHABHttpClient.BaseUrl = settings.OpenHABUrl;
            OpenHABHttpClient.ResetClient();
        }

        /// <inheritdoc />
        public async Task<OpenHABVersion> GetOpenHABVersion()
        {
            try
            {
                var result = await OpenHABHttpClient.Client().GetAsync(Constants.Api.ServerVersion).ConfigureAwait(false);
                return !result.IsSuccessStatusCode ? OpenHABVersion.One : OpenHABVersion.Two;
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version)
        {
            try
            {
                var result = await OpenHABHttpClient.Client().GetAsync(Constants.Api.Sitemaps).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                // V1 = xml
                if (version == OpenHABVersion.One)
                {
                    var sitemaps = new List<OpenHABSitemap>();
                    XDocument xml = XDocument.Parse(resultString);
                    foreach (XElement xElement in xml.Element("sitemaps").Elements())
                    {
                        var sitemap = new OpenHABSitemap(xElement);
                        sitemaps.Add(sitemap);
                    }

                    return sitemaps;
                }

                // V2 = JSON
                return JsonConvert.DeserializeObject<List<OpenHABSitemap>>(resultString);
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<OpenHABWidget>> LoadItemsFromSitemap(OpenHABSitemap sitemap, OpenHABVersion version)
        {
            try
            {
                var result = await OpenHABHttpClient.Client().GetAsync(sitemap.Link).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                // V1 = xml
                if (version == OpenHABVersion.One)
                {
                    var widgets = ParseWidgets(resultString);
                    return widgets;
                }

                // V2 = JSON
                return JsonConvert.DeserializeObject<List<OpenHABWidget>>(resultString);
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task SendCommand(OpenHABItem item, string command)
        {
            try
            {
                var client = OpenHABHttpClient.Client();
                var content = new StringContent(command);
                var result = await client.PostAsync(item.Link, content);

                if (!result.IsSuccessStatusCode)
                {
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        private ICollection<OpenHABWidget> ParseWidgets(string resultString)
        {
            var xml = XDocument.Parse(resultString);

            return xml.Element("sitemap").Element("homepage").Elements("widget").Select(xElement => new OpenHABWidget(xElement)).ToList();
        }
    }
}