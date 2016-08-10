using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Openhab.Model;

namespace Openhab.Core.SDK
{
    public class OpenHAB : IOpenHAB
    {
        public async Task<OpenHABVersion> GetOpenHABVersion()
        {
            try
            {
                var result = await OpenHABHttpClient.Client().GetAsync("rest/bindings").ConfigureAwait(false);
                return !result.IsSuccessStatusCode ? OpenHABVersion.One : OpenHABVersion.Two;
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        public async Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version)
        {
            try
            {
                var result = await OpenHABHttpClient.Client().GetAsync("rest/sitemaps").ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (version == OpenHABVersion.One) // V1 = xml
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

                return JsonConvert.DeserializeObject<List<OpenHABSitemap>>(resultString); //V2 = JSON
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

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

                if (version == OpenHABVersion.One) // V1 = xml
                {
                    var widgets = ParseWidgets(resultString);
                    return widgets;
                }

                return JsonConvert.DeserializeObject<List<OpenHABWidget>>(resultString); //V2 = JSON
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
        }

        private ICollection<OpenHABWidget> ParseWidgets(string resultString)
        {
            var widgets = new List<OpenHABWidget>();
            XDocument xml = XDocument.Parse(resultString);
            foreach (XElement xElement in xml.Element("sitemap").Element("homepage").Elements("widget"))
            {
                var widget = new OpenHABWidget(xElement);
                widgets.Add(widget);
            }

            return widgets;
        }
    }
}