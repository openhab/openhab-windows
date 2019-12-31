using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Connectivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Messages;
using OpenHAB.Core.Model;

namespace OpenHAB.Core.SDK
{
    /// <summary>
    /// The main SDK implementation of the connection to OpenHAB.
    /// </summary>
    public class OpenHABClient : IOpenHAB
    {
        private readonly IMessenger _messenger;
        private readonly ILogger<OpenHABClient> _logger;
        private readonly ISettingsService _settingsService;
        private OpenHABHttpClientType _connectionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABClient"/> class.
        /// </summary>
        /// <param name="settingsService">The service to fetch the settings.</param>
        /// <param name="messenger">The messenger instance.</param>
        public OpenHABClient(ISettingsService settingsService, IMessenger messenger, ILogger<OpenHABClient> logger)
        {
            _settingsService = settingsService;
            _messenger = messenger;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckUrlReachability(string openHABUrl, OpenHABHttpClientType connectionType)
        {
            if (string.IsNullOrWhiteSpace(openHABUrl))
            {
                return false;
            }

            if (!openHABUrl.EndsWith("/"))
            {
                openHABUrl = openHABUrl + "/";
            }

            try
            {
                Settings settings = _settingsService.Load();
                var client = OpenHABHttpClient.DisposableClient(connectionType, settings);
                var result = await client.GetAsync(openHABUrl + "rest").ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed.");
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<OpenHABVersion> GetOpenHABVersion()
        {
            try
            {
                var settings = _settingsService.Load();
                var httpClient = OpenHABHttpClient.Client(_connectionType, settings);

                if (httpClient == null)
                {
                    return OpenHABVersion.None;
                }

                var result = await httpClient.GetAsync(Constants.Api.ServerVersion).ConfigureAwait(false);
                _settingsService.ServerVersion = !result.IsSuccessStatusCode ? OpenHABVersion.One : OpenHABVersion.Two;
                return _settingsService.ServerVersion;
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
                var settings = _settingsService.Load();
                var result = await OpenHABHttpClient.Client(_connectionType, settings).GetAsync(sitemap.Link).ConfigureAwait(false);
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
                var jsonObject = JObject.Parse(resultString);
                return JsonConvert.DeserializeObject<List<OpenHABWidget>>(jsonObject["homepage"]["widgets"].ToString());
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadItemsFromSitemap failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<ICollection<OpenHABSitemap>> LoadSiteMaps(OpenHABVersion version, List<Func<OpenHABSitemap, bool>> filters)
        {
            try
            {
                var settings = _settingsService.Load();
                var result = await OpenHABHttpClient.Client(_connectionType, settings).GetAsync(Constants.Api.Sitemaps).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                var sitemaps = new List<OpenHABSitemap>();

                // V1 = xml
                if (version == OpenHABVersion.One)
                {
                    XDocument xml = XDocument.Parse(resultString);

                    foreach (XElement xElement in xml.Element("sitemaps").Elements())
                    {
                        var sitemap = new OpenHABSitemap(xElement);
                        sitemaps.Add(sitemap);
                    }

                    return sitemaps;
                }

                // V2 = JSON
                sitemaps = JsonConvert.DeserializeObject<List<OpenHABSitemap>>(resultString);

                return sitemaps.Where(sitemap =>
                {
                    bool isIncluded = true;
                    filters.ForEach(filter => isIncluded &= filter(sitemap));

                    return isIncluded;
                }).ToList();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "LoadSiteMaps failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LoadSiteMaps failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async Task<bool> ResetConnection()
        {
            var settings = _settingsService.Load();
            bool isValid = await SetValidUrl(settings);

            if (!isValid)
            {
                return false;
            }

            OpenHABHttpClient.ResetClient();

            return true;
        }

        /// <inheritdoc />
        public async Task SendCommand(OpenHABItem item, string command)
        {
            try
            {
                var settings = _settingsService.Load();
                var client = OpenHABHttpClient.Client(_connectionType, settings);
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

        /// <inheritdoc />
        public async void StartItemUpdates()
        {
            await Task.Run(async () =>
            {
                var settings = _settingsService.Load();
                var client = OpenHABHttpClient.Client(_connectionType, settings);
                var requestUri = Constants.Api.Events;

                try
                {
                    var stream = await client.GetStreamAsync(requestUri);

                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var updateEvent = reader.ReadLine();
                            if (updateEvent?.StartsWith("data:") == true)
                            {
                                var data = JsonConvert.DeserializeObject<EventStreamData>(updateEvent.Remove(0, 6));
                                if (!data.Topic.EndsWith("state"))
                                {
                                    continue;
                                }

                                var payload = JsonConvert.DeserializeObject<EventStreamPayload>(data.Payload);
                                _messenger.Send(new UpdateItemMessage(data.Topic.Replace("smarthome/items/", string.Empty).Replace("/state", string.Empty), payload.Value));
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "StartItemUpdates failed.");
                    throw new OpenHABException("Fetching item updates failed", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "StartItemUpdates failed.");
                    throw new OpenHABException("Fetching item updates failed", ex);
                }
            }).ConfigureAwait(false);
        }

        private ICollection<OpenHABWidget> ParseWidgets(string resultString)
        {
            var xml = XDocument.Parse(resultString);

            return
                xml.Element("sitemap")
                    .Element("homepage")
                    .Elements("widget")
                    .Select(xElement => new OpenHABWidget(xElement))
                    .ToList();
        }

        private async Task<bool> SetValidUrl(Settings settings)
        {
            var isRunningInDemoMode = settings.IsRunningInDemoMode != null && settings.IsRunningInDemoMode.Value;

            // no url configured yet
            if (string.IsNullOrWhiteSpace(settings.LocalConnection.Url) &&
                string.IsNullOrWhiteSpace(settings.RemoteConnection.Url) &&
                !isRunningInDemoMode)
            {
                return false;
            }

            if (isRunningInDemoMode)
            {
                OpenHABHttpClient.BaseUrl = Constants.Api.DemoModeUrl;
                return true;
            }

            if (NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection)
            {
                if (string.IsNullOrEmpty(settings.RemoteConnection.Url.Trim()))
                {
                    throw new OpenHABException("No remote url configured");
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                _connectionType = OpenHABHttpClientType.Remote;
                return true;
            }

            bool isReachable = await CheckUrlReachability(settings.LocalConnection.Url, OpenHABHttpClientType.Local).ConfigureAwait(false);
            if (isReachable)
            {
                OpenHABHttpClient.BaseUrl = settings.LocalConnection.Url;
                _connectionType = OpenHABHttpClientType.Local;

                return true;
            }
            else
            {
                // If remote URL is configured
                if (!string.IsNullOrWhiteSpace(settings.RemoteConnection.Url) &&
                    await CheckUrlReachability(settings.RemoteConnection.Url, OpenHABHttpClientType.Remote).ConfigureAwait(false))
                {
                    OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                    _connectionType = OpenHABHttpClientType.Remote;
                    return true;
                }

                Messenger.Default.Send<FireInfoMessage>(new FireInfoMessage(MessageType.NotReachable));
            }

            return false;
        }
    }
}