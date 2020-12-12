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
using OpenHAB.Core.Model.Connection;

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
        private OpenHABConnection _connection;
        private OpenHABHttpClient _openHABHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABClient"/> class.
        /// </summary>
        /// <param name="settingsService">The service to fetch the settings.</param>
        /// <param name="messenger">The messenger instance.</param>
        /// <param name="logger">Logger class.</param>
        /// <param name="openHABHttpClient">OpenHab Http client factory.</param>
        public OpenHABClient(ISettingsService settingsService, IMessenger messenger, ILogger<OpenHABClient> logger, OpenHABHttpClient openHABHttpClient)
        {
            _settingsService = settingsService;
            _messenger = messenger;
            _logger = logger;
            _openHABHttpClient = openHABHttpClient;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckUrlReachability(OpenHABConnection connection)
        {
            if (string.IsNullOrWhiteSpace(connection?.Url))
            {
                return false;
            }

            if (!connection.Url.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Url = connection.Url + "/";
            }

            try
            {
                Settings settings = _settingsService.Load();
                var client = _openHABHttpClient.DisposableClient(connection, settings);
                var result = await client.GetAsync(connection.Url + "rest").ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed");

                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "CheckUrlReachability failed");

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
                var httpClient = _openHABHttpClient.Client(_connection, settings);

                if (httpClient == null)
                {
                    return OpenHABVersion.None;
                }

                var result = await httpClient.GetAsync(Constants.Api.ServerVersion).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request get OpenHab version failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

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
                _logger.LogInformation($"Load sitemaps items for sitemap '{sitemap.Name}'");

                var settings = _settingsService.Load();
                var result = await _openHABHttpClient.Client(_connection, settings).GetAsync(sitemap.Link).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for loading sitemaps items failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                ICollection<OpenHABWidget> items = null;
                if (version == OpenHABVersion.One)
                {
                    // V1 = xml
                    items = ParseWidgets(resultString);
                }
                else
                {
                    // V2 = JSON
                    var jsonObject = JObject.Parse(resultString);
                    items = JsonConvert.DeserializeObject<List<OpenHABWidget>>(jsonObject["homepage"]["widgets"].ToString());
                }

                _logger.LogInformation($"Loaded '{items.Count}' sitemaps items from server");

                return items;
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
                _logger.LogInformation($"Load sitemaps for OpenHab server version '{version.ToString()}'");

                var settings = _settingsService.Load();
                var result = await _openHABHttpClient.Client(_connection, settings).GetAsync(Constants.Api.Sitemaps).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for loading sitemaps failed, ErrorCode:'{result.StatusCode}'");
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

                _logger.LogInformation($"Loaded '{sitemaps.Count}' sitemaps from server");
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

            _openHABHttpClient.ResetClient();

            return true;
        }

        /// <inheritdoc />
        public async Task SendCommand(OpenHABItem item, string command)
        {
            try
            {
                _logger.LogInformation($"Send Command '{command}' for item '{item.Name} of type '{item.Type}'");

                var settings = _settingsService.Load();
                var client = _openHABHttpClient.Client(_connection, settings);
                var content = new StringContent(command);

                var result = await client.PostAsync(item.Link, content);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request for command failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "SendCommand failed.");
                throw new OpenHABException("Invalid call", ex);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "SendCommand failed.");
                throw new OpenHABException("Invalid call", ex);
            }
        }

        /// <inheritdoc />
        public async void StartItemUpdates(System.Threading.CancellationToken token)
        {
            await Task.Run(async () =>
            {
                var settings = _settingsService.Load();
                var client = _openHABHttpClient.Client(_connection, settings);
                var requestUri = Constants.Api.Events;

                _logger.LogInformation($"Retrive item updates from '{client.BaseAddress.ToString()}'");

                try
                {
                    var stream = await client.GetStreamAsync(requestUri).ConfigureAwait(false);

                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            var updateEvent = reader.ReadLine();
                            if (updateEvent?.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase) == true)
                            {
                                var data = JsonConvert.DeserializeObject<EventStreamData>(updateEvent.Remove(0, 6));
                                if (!data.Topic.EndsWith("state", StringComparison.InvariantCultureIgnoreCase))
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
                    .Select(xElement => OpenHABWidgetFactory.Parse(xElement))
                    .ToList();
        }

        private async Task<bool> SetValidUrl(Settings settings)
        {
            _logger.LogInformation("Validate Url");

            var isRunningInDemoMode = settings.IsRunningInDemoMode != null && settings.IsRunningInDemoMode.Value;

            _logger.LogInformation($"App is running in demo mode: {isRunningInDemoMode}");

            // no url configured yet
            if (string.IsNullOrWhiteSpace(settings.LocalConnection?.Url) &&
                string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url) &&
                !isRunningInDemoMode)
            {
                return false;
            }

            if (isRunningInDemoMode)
            {
                _connection = new DemoConnectionProfile().CreateConnection();
                OpenHABHttpClient.BaseUrl = Constants.Api.DemoModeUrl;
                return true;
            }

            bool meteredConnection = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection;
            _logger.LogInformation($"Metered Connection Type: {meteredConnection}");

            if (meteredConnection)
            {
                if (string.IsNullOrEmpty(settings.RemoteConnection.Url.Trim()))
                {
                    string message = "No remote url configured";
                    _logger.LogWarning(message);

                    throw new OpenHABException(message);
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                _connection = settings.RemoteConnection;

                return true;
            }

            bool isReachable = await CheckUrlReachability(settings.LocalConnection).ConfigureAwait(false);
            _logger.LogInformation($"OpenHab server is reachable: {isReachable}");

            if (isReachable)
            {
                OpenHABHttpClient.BaseUrl = settings.LocalConnection.Url;
                _connection = settings.LocalConnection;

                return true;
            }
            else
            {
                // If remote URL is configured
                if (!string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url) &&
                    await CheckUrlReachability(settings.RemoteConnection).ConfigureAwait(false))
                {
                    OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                    _connection = settings.RemoteConnection;
                    return true;
                }

                Messenger.Default.Send<FireInfoMessage>(new FireInfoMessage(MessageType.NotReachable));
            }

            _logger.LogWarning($"OpenHab server url is not valid");

            return false;
        }
    }
}