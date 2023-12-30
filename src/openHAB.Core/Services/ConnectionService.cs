using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using openHAB.Core.Common;
using openHAB.Core.Connection;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Services
{
    /// <inheritdoc/>
    public class ConnectionService : IConnectionService
    {
        private readonly ILogger<ConnectionService> _logger;
        private readonly ISettingsService _settingsService;
        private readonly OpenHABHttpClient _openHABHttpClient;

        public ConnectionService(ISettingsService settingsService, OpenHABHttpClient openHABHttpClient, ILogger<ConnectionService> logger)
        {
            _logger = logger;
            _settingsService = settingsService;
            _openHABHttpClient = openHABHttpClient;
        }

        /// <inheritdoc/>
        public async Task<OpenHABConnection> DetectAndRetriveConnection(Settings settings)
        {
            _logger.LogInformation("Validate Url");

            var isRunningInDemoMode = settings.IsRunningInDemoMode != null && settings.IsRunningInDemoMode.Value;

            _logger.LogInformation($"App is running in demo mode: {isRunningInDemoMode}");

            // no url configured yet
            if (string.IsNullOrWhiteSpace(settings.LocalConnection?.Url) &&
                string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url) &&
                !isRunningInDemoMode)
            {
                return null;
            }

            if (isRunningInDemoMode)
            {
                OpenHABConnection connection = new DemoConnectionProfile().CreateConnection();
                OpenHABHttpClient.BaseUrl = Constants.API.DemoModeUrl;
                return connection;
            }

            bool meteredConnection = NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection;
            _logger.LogInformation($"Metered Connection Type: {meteredConnection}");

            if (meteredConnection)
            {
                if (string.IsNullOrEmpty(settings.RemoteConnection?.Url))
                {
                    string message = "No remote url configured";
                    _logger.LogWarning(message);

                    return null;
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                OpenHABConnection connection = settings.RemoteConnection;

                return connection;
            }

            HttpResponseResult<bool> result = await CheckUrlReachability(settings.LocalConnection).ConfigureAwait(false);
            _logger.LogInformation($"OpenHab server is reachable: {result.Content}");

            if (result.Content)
            {
                OpenHABHttpClient.BaseUrl = settings.LocalConnection.Url;
                OpenHABConnection connection = settings.LocalConnection;

                return connection;
            }
            else
            {
                // If remote URL is configured
                if (string.IsNullOrWhiteSpace(settings.RemoteConnection?.Url))
                {
                    StrongReferenceMessenger.Default.Send<FireErrorMessage>(new FireErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return null;
                }

                result = await CheckUrlReachability(settings.RemoteConnection).ConfigureAwait(false);
                if (!result.Content)
                {
                    StrongReferenceMessenger.Default.Send<FireErrorMessage>(new FireErrorMessage(AppResources.Errors.GetString("ConnectionTestFailed")));
                    _logger.LogWarning($"OpenHab server url is not valid");

                    return null;
                }

                OpenHABHttpClient.BaseUrl = settings.RemoteConnection.Url;
                OpenHABConnection connection = settings.RemoteConnection;

                return connection;
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseResult<bool>> CheckUrlReachability(OpenHABConnection connection)
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

        /// <inheritdoc />
        public async Task<HttpResponseResult<ServerInfo>> GetOpenHABServerInfo(OpenHABConnection connection)
        {
            try
            {
                var settings = _settingsService.Load();
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

                OpenHABAPIInfo apiInfo = JsonConvert.DeserializeObject<OpenHABAPIInfo>(responseBody);
                if (apiInfo.Version < 4)
                {
                    serverInfo.Version = OpenHABVersion.Two;
                    return new HttpResponseResult<ServerInfo>(serverInfo, result.StatusCode);
                }

                string runtimeversion = Regex.Replace(apiInfo?.RuntimeInfo.Version, "[^.0-9]", string.Empty);
                if (!Version.TryParse(runtimeversion, out Version serverVersion))
                {
                    string message = "Not able to parse runtime verion from openHAB server";
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
