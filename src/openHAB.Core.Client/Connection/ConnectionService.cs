using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.Extensions.Logging;
using openHAB.Common;
using openHAB.Core.Client.Common;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;

namespace openHAB.Core.Client.Connection;


/// <summary>
/// <inheritdoc/>
public class ConnectionService : IConnectionService
{
    private readonly ILogger<ConnectionService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private Models.Connection _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionService"/> class.
    /// </summary>
    /// <param name="openHABHttpClient">The OpenHAB HTTP client.</param>
    /// <param name="logger">The logger.</param>
    public ConnectionService(IHttpClientFactory httpClientFactory, ILogger<ConnectionService> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public Models.Connection CurrentConnection => _connection;

    /// <inheritdoc/>
    public async Task<HttpResponseResult<bool>> CheckUrlReachability(Models.Connection connection)
    {
        if (string.IsNullOrWhiteSpace(connection?.Url))
        {
            return new HttpResponseResult<bool>(false, null);
        }

        if (!connection.Url.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
        {
            connection.Url += "/";
        }

        HttpResponseResult<ServerInfo> result = await GetOpenHABServerInfo(connection).ConfigureAwait(false);
        return result.Content == null
            ? new HttpResponseResult<bool>(false, null, result.Exception)
            : new HttpResponseResult<bool>(true, result.StatusCode);
    }

    /// <inheritdoc/>
    public async Task<Models.Connection> DetectAndRetrieveConnection(Models.Connection localConnection, Models.Connection remoteConnection, bool isRunningInDemoMode)
    {
        _logger.LogInformation("Validate Connection");
        _logger.LogInformation("APP is running in demo mode: {IsRunningInDemoMode}", isRunningInDemoMode);

        bool noConnectionConfigured =
            string.IsNullOrWhiteSpace(localConnection?.Url) &&
            string.IsNullOrWhiteSpace(remoteConnection?.Url);

        if (isRunningInDemoMode || noConnectionConfigured)
        {
            _logger.LogInformation("No connection configured or demo mode active -> using demo connection");
            _connection = new DemoConnectionProfile().CreateConnection();
            return _connection;
        }

        bool meteredConnection = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection;
        _logger.LogInformation("Metered Connection Type: {MeteredConnection}", meteredConnection);

        if (meteredConnection)
        {
            if (string.IsNullOrEmpty(remoteConnection?.Url))
            {
                _logger.LogWarning("No remote url configured");
                return null;
            }

            _connection = remoteConnection;
            return _connection;
        }

        HttpResponseResult<bool> result = await CheckUrlReachability(localConnection).ConfigureAwait(false);
        _logger.LogInformation("OpenHab server is reachable: {IsReachable}", result.Content);

        if (result.Content)
        {
            _connection = localConnection;
            return _connection;
        }

        if (string.IsNullOrWhiteSpace(remoteConnection?.Url))
        {
            StrongReferenceMessenger.Default.Send(new ConnectionErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
            _logger.LogWarning("OpenHab server url is not valid");
            return null;
        }

        result = await CheckUrlReachability(remoteConnection).ConfigureAwait(false);
        if (!result.Content)
        {
            StrongReferenceMessenger.Default.Send(new ConnectionErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
            _logger.LogWarning("OpenHab server url is not valid");
            return null;
        }

        _connection = remoteConnection;
        return _connection;
    }

    /// <inheritdoc />
    public async Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo(Models.Connection connection)
    {
        try
        {
            HttpClient httpClient = connection.Type switch
            {
                HttpClientType.Local => _httpClientFactory.CreateClient("local"),
                HttpClientType.Remote => _httpClientFactory.CreateClient("remote"),
                _ => throw new OpenHABException("Invalid connection type")
            };

            HttpResponseMessage result = await httpClient.GetAsync(Constants.API.ServerInformation).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP request get OpenHab version failed, ErrorCode:'{StatusCode}'", result.StatusCode);
                throw new OpenHABException($"{result.StatusCode} received from server");
            }

            string responseBody = await result.Content.ReadAsStringAsync();
            APIInfo apiInfo = JsonSerializer.Deserialize<APIInfo>(responseBody);

            if (int.TryParse(apiInfo.Version, out int apiVersion) && apiVersion < 4)
            {
                return new HttpResponseResult<ServerInfo>(new ServerInfo { Version = OpenHABVersion.Two }, result.StatusCode);
            }

            string runtimeversion = Regex.Replace(apiInfo?.RuntimeInfo.Version, "[^.0-9]", string.Empty, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
            if (!Version.TryParse(runtimeversion, out Version serverVersion))
            {
                string message = "Not able to parse runtime version from openHAB server";
                _logger.LogError(message);
                throw new OpenHABException(message);
            }

            return new HttpResponseResult<ServerInfo>(new ServerInfo
            {
                Version = (OpenHABVersion)serverVersion.Major,
                RuntimeVersion = apiInfo?.RuntimeInfo.Version,
                Build = apiInfo.RuntimeInfo.BuildString
            }, result.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOpenHABServerInfo failed.");
            return new HttpResponseResult<ServerInfo>(null, null, ex);
        }
    }
}
