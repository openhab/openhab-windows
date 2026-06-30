using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using openHAB.Core.Client.Common;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Contracts;
using openHAB.Core.Client.Event;
using openHAB.Core.Client.Event.Contracts;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;

namespace openHAB.Core.Client;


/// <summary>
/// The main SDK implementation of the connection to OpenHAB.
/// </summary>
public class OpenHABClient : IOpenHABClient
{
    private readonly IConnectionService _connectionService;
    private readonly IOpenHABEventParser _eventParser;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OpenHABClient> _logger;
    private readonly IMessenger _messenger;
    private Connection.Models.Connection _connection;
    private HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenHABClient"/> class.
    /// </summary>
    /// <param name="settingsService">The service to fetch the settings.</param>
    /// <param name="messenger">The messenger instance.</param>
    /// <param name="eventParser">openHAB event parser.</param>
    /// <param name="connectionService"></param>
    /// <param name="logger">Logger class.</param>
    /// <param name="openHABHttpClient">OpenHab HTTP client factory.</param>
    public OpenHABClient(
        IMessenger messenger,
        IOpenHABEventParser eventParser,
        IConnectionService connectionService,
        ILogger<OpenHABClient> logger,
        IHttpClientFactory httpClientFactory)
    {
        _messenger = messenger;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _eventParser = eventParser;
        _connectionService = connectionService;
    }

    /// <inheritdoc />
    public async Task<Item> GetItemByName(string itemName)
    {
        try
        {
            _logger.LogInformation("Load item by name '{ItemName}' from openHAB server", itemName);

            Uri resourceUrl = new Uri($"{Constants.API.Items}/{itemName}", UriKind.Relative);

            HttpResponseMessage result = await _httpClient.GetAsync(resourceUrl).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request to fetch item '{ItemName}' failed, ErrorCode:'{StatusCode}'", itemName, result.StatusCode);
                throw new OpenHABException($"{result.StatusCode} received from server");
            }

            string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            Item item = JsonSerializer.Deserialize<Item>(resultString, serializerOptions);

            _logger.LogInformation("Loaded item '{ItemName}' from server", itemName);

            return item;
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
    public async Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo()
    {
        return await _connectionService.GetOpenHABServerInfo(_connection);
    }

    public async Task<Sitemap> GetSitemap(string sitemapLink, OpenHABVersion version)
    {
        try
        {
            _logger.LogInformation("Get sitemap by link '{SitemapLink}'", sitemapLink);

            HttpResponseMessage result = await _httpClient.GetAsync(sitemapLink).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request for loading sitemap failed, ErrorCode:'{StatusCode}'", result.StatusCode);
                throw new OpenHABException($"{result.StatusCode} received from server");
            }

            string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Sitemap sitemap = null;
            if (version >= OpenHABVersion.Two)
            {
                JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                };

                sitemap = JsonSerializer.Deserialize<Sitemap>(resultString, serializerOptions);
            }
            else
            {
                string message = "openHAB version is not supported.";
                _logger.LogError(message);
                throw new OpenHABException(message);
            }

            _logger.LogInformation("Loaded '{SitemapName}' sitemap successfully from server", sitemap.Name);

            return sitemap;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "GetSitemap failed.");
            throw new OpenHABException("Invalid call", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetSitemap failed.");
            throw new OpenHABException("Invalid call", ex);
        }
    }

    /// <inheritdoc />
    public async Task<ICollection<Widget>> LoadItemsFromSitemap(string sitemapLink, OpenHABVersion version)
    {
        try
        {
            _logger.LogInformation("Load site map items for site map '{SiteMapLink}'", sitemapLink);

            HttpResponseMessage result = await _httpClient.GetAsync(sitemapLink).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request for loading site map items failed, ErrorCode:'{StatusCode}'", result.StatusCode);
                throw new OpenHABException($"{result.StatusCode} received from server");
            }

            string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (version < OpenHABVersion.Two)
            {
                string message = "openHAB version is not supported.";
                _logger.LogError(message);
                throw new OpenHABException(message);
            }

            JsonSerializerOptions serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };

            Sitemap sitemap = JsonSerializer.Deserialize<Sitemap>(resultString, serializerOptions);
            ICollection<Widget> items = sitemap?.Homepage?.Widgets ?? new List<Widget>();

            _logger.LogInformation("Loaded '{ItemCount}' site map items from server", items.Count);

            return items;
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "LoadItemsFromSiteMap failed.");
            throw new OpenHABException("Invalid call", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadItemsFromSiteMap failed.");
            throw new OpenHABException("Invalid call", ex);
        }
    }

    /// <inheritdoc />
    public async Task<ICollection<Sitemap>> LoadSitemaps(OpenHABVersion version, List<Func<Sitemap, bool>> filters)
    {
        try
        {
            _logger.LogInformation("Load sitemaps for OpenHab server version '{Version}'", version.ToString());

            HttpResponseMessage result = await _httpClient.GetAsync(Constants.API.Sitemaps).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request for loading sitemaps failed, ErrorCode:'{StatusCode}'", result.StatusCode);
                throw new OpenHABException($"{result.StatusCode} received from server");
            }

            string resultString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            List<Sitemap> sitemaps = JsonSerializer.Deserialize<List<Sitemap>>(resultString);
            if (sitemaps == null)
            {
                _logger.LogError("Failed to load sitemaps from server");
                return new List<Sitemap>();
            }

            _logger.LogInformation("Loaded '{SitemapsCount}' sitemaps from server", sitemaps.Count);
            return sitemaps.Where(sitemap =>
            {
                bool isIncluded = true;
                filters.ForEach(filter => isIncluded &= filter(sitemap));

                return isIncluded;
            }).ToList();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "LoadSitemaps failed.");
            throw new OpenHABException("Invalid call", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LoadSitemaps failed.");
            throw new OpenHABException("Invalid call", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ResetConnection(Connection.Models.Connection localConnection, Connection.Models.Connection remoteConnection, bool? isRunningInDemoMode)
    {
        bool isDemoMode = isRunningInDemoMode.HasValue && isRunningInDemoMode.Value;
        Connection.Models.Connection connection = await _connectionService.DetectAndRetrieveConnection(localConnection, remoteConnection, isDemoMode);
        if (connection == null)
        {
            return false;
        }

        switch (connection.Type)
        {
            case HttpClientType.Local:
                _logger.LogInformation("Initialize local connection to {Url}", connection.Url);
                _httpClient = _httpClientFactory.CreateClient("local");
                break;
            case HttpClientType.Remote:
                _logger.LogInformation("Initialize remote connection to {Url}", connection.Url);
                _httpClient = _httpClientFactory.CreateClient("remote");
                break;
        }

        _connection = connection;
        return true;
    }

    /// <inheritdoc />
    public async Task<HttpResponseResult<bool>> SendCommand(Item item, string command)
    {
        try
        {
            if (item == null)
            {
                _logger.LogInformation("Send command is canceled, because item is null (e.g. widget not linked to item)");
                return new HttpResponseResult<bool>(false, null, null);
            }

            _logger.LogInformation("Send Command '{Command}' for item '{ItemName}' of type '{ItemType}'", command, item.Name, item.Type);
            StringContent content = new StringContent(command);

            var result = await _httpClient.PostAsync(item.Link, content).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request for command failed, ErrorCode:'{StatusCode}'", result.StatusCode);
            }

            return new HttpResponseResult<bool>(result.IsSuccessStatusCode, result.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "SendCommand failed.");
            return new HttpResponseResult<bool>(false, null, ex);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError(ex, "SendCommand failed.");
            return new HttpResponseResult<bool>(false, null, ex);
        }
    }

    /// <inheritdoc />
    public async void StartItemUpdates(System.Threading.CancellationToken token)
    {
        string requestUri = Constants.API.Events;

        _logger.LogInformation("Retrieve item updates from '{BaseAddress}'", _httpClient.BaseAddress.ToString());

        try
        {
            Stream stream = await _httpClient.GetStreamAsync(requestUri).ConfigureAwait(false);

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    string updateEvent = await reader.ReadLineAsync().ConfigureAwait(false);
                    if (updateEvent?.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        OpenHABEvent ohEvent = _eventParser.Parse(updateEvent);
                        if (ohEvent == null)
                        {
                            continue;
                        }

                        switch (ohEvent.EventType)
                        {
                            case OpenHABEventType.ItemStateEvent:
                                _messenger.Send(new UpdateItemMessage(ohEvent.ItemName, ohEvent.Value));
                                break;
                            case OpenHABEventType.ThingUpdatedEvent:
                                break;
                            case OpenHABEventType.RuleStatusInfoEvent:
                                break;
                            case OpenHABEventType.ItemStatePredictedEvent:
                                break;
                            case OpenHABEventType.GroupItemStateChangedEvent:
                                break;
                            case OpenHABEventType.ItemStateChangedEvent:
                                _messenger.Send(new ItemStateChangedMessage(ohEvent.ItemName, ohEvent.Value, ohEvent.OldValue));
                                break;
                            case OpenHABEventType.Unknown:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "StartItemUpdates failed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartItemUpdates failed.");
        }
    }
}
