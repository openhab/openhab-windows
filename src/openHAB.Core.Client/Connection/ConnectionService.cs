using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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

namespace openHAB.Core.Client.Connection
{
    /// <inheritdoc/>
    public class ConnectionService : IConnectionService
    {
        private readonly ILogger<ConnectionService> _logger;
        private readonly OpenHABHttpClient _openHABHttpClient;
        private Models.Connection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionService"/> class.
        /// </summary>
        /// <param name="openHABHttpClient">The OpenHAB HTTP client.</param>
        /// <param name="logger">The logger.</param>
        public ConnectionService(OpenHABHttpClient openHABHttpClient, ILogger<ConnectionService> logger)
        {
            _logger = logger;
            _openHABHttpClient = openHABHttpClient;
        }

        /// <inheritdoc/>
        public Models.Connection CurrentConnection
        {
            get => _connection;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseResult<bool>> CheckUrlReachability(Models.Connection connection)
        {
            if (string.IsNullOrWhiteSpace(connection?.Url))
            {
                return new HttpResponseResult<bool>(false, null);
            }

            if (!connection.Url.EndsWith("/", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Url = connection.Url + "/";
            }

            OpenHABHttpClient.BaseUrl = connection.Url;
            HttpResponseResult<ServerInfo> result = await GetOpenHABServerInfo(connection).ConfigureAwait(false);
            if (result.Content == null)
            {
                return new HttpResponseResult<bool>(false, null, result.Exception);
            }
            else
            {
                return new HttpResponseResult<bool>(true, result.StatusCode);
            }
        }

        /// <inheritdoc/>
        public async Task<Models.Connection> DetectAndRetriveConnection(Models.Connection localConnection, Models.Connection remoteConnection, bool isRunningInDemoMode)
        {
            _logger.LogInformation("Validate Connection");
            _logger.LogInformation($"App is running in demo mode: {isRunningInDemoMode}");

            // no url configured yet
            if (string.IsNullOrWhiteSpace(localConnection?.Url) &&
                string.IsNullOrWhiteSpace(remoteConnection?.Url) &&
                !isRunningInDemoMode)
            {
                return null;
            }

            if (isRunningInDemoMode)
            {
                _connection = new DemoConnectionProfile().CreateConnection();
                OpenHABHttpClient.BaseUrl = Constants.API.DemoModeUrl;
                return _connection;
            }

            bool meteredConnection = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection;
            _logger.LogInformation($"Metered Connection Type: {meteredConnection}");

            if (meteredConnection)
            {
                if (string.IsNullOrEmpty(remoteConnection?.Url))
                {
                    string message = "No remote url configured";
                    _logger.LogWarning(message);

                    return null;
                }

                OpenHABHttpClient.BaseUrl = remoteConnection.Url;
                _connection = remoteConnection;

                return _connection;
            }

            HttpResponseResult<bool> result = await CheckUrlReachability(localConnection).ConfigureAwait(false);
            _logger.LogInformation($"OpenHab server is reachable: {result.Content}");

            if (result.Content)
            {
                OpenHABHttpClient.BaseUrl = localConnection.Url;
                _connection = localConnection;

                return _connection;
            }
            else
            {
                // If remote URL is configured
                if (string.IsNullOrWhiteSpace(remoteConnection?.Url))
                {
                    StrongReferenceMessenger.Default.Send<ConnectionErrorMessage>(new ConnectionErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return null;
                }

                result = await CheckUrlReachability(remoteConnection).ConfigureAwait(false);
                if (!result.Content)
                {
                    StrongReferenceMessenger.Default.Send<ConnectionErrorMessage>(new ConnectionErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return null;
                }

                OpenHABHttpClient.BaseUrl = remoteConnection.Url;
                _connection = remoteConnection;

                return _connection;
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo(Models.Connection connection)
        {
            try
            {
                var httpClient = _openHABHttpClient.DisposableClient(connection);
                httpClient.BaseAddress = new Uri(connection.Url);

                ServerInfo serverInfo = new ServerInfo();

                HttpResponseMessage result = await httpClient.GetAsync(Constants.API.ServerInformation).ConfigureAwait(false);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http request get OpenHab version failed, ErrorCode:'{result.StatusCode}'");
                    throw new OpenHABException($"{result.StatusCode} received from server");
                }

                string responseBody = await result.Content.ReadAsStringAsync();

                APIInfo apiInfo = JsonSerializer.Deserialize<APIInfo>(responseBody);
                
                if (int.TryParse(apiInfo.Version, out int apiVersion) && apiVersion < 4)
                {
                    serverInfo.Version = OpenHABVersion.Two;
                    return new HttpResponseResult<ServerInfo>(serverInfo, result.StatusCode);
                }

                string runtimeversion = Regex.Replace(apiInfo?.RuntimeInfo.Version, "[^.0-9]", string.Empty, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
                if (!Version.TryParse(runtimeversion, out Version serverVersion))
                {
                    string message = "Not able to parse runtime version from openHAB server";
                    _logger.LogError(message);

                    throw new OpenHABException(message);
                }

                OpenHABVersion openHABVersion = (OpenHABVersion)serverVersion.Major;

                serverInfo.Version = openHABVersion;
                serverInfo.RuntimeVersion = apiInfo?.RuntimeInfo.Version;
                serverInfo.Build = apiInfo.RuntimeInfo.BuildString;

                return new HttpResponseResult<ServerInfo>(serverInfo, result.StatusCode);
            }
            catch (ArgumentNullException ex)
            {
                throw new OpenHABException("Invalid call", ex);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "GetOpenHABServerInfo failed");

                return new HttpResponseResult<ServerInfo>(null, null, ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetOpenHABServerInfo failed");

                return new HttpResponseResult<ServerInfo>(null, null, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOpenHABServerInfo failed.");
                return new HttpResponseResult<ServerInfo>(null, null, ex);
            }
        }
    }
}
