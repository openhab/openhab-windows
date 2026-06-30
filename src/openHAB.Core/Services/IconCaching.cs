using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Models;
using openHAB.Core.Client.Options;
using openHAB.Core.Services.Contracts;
using openHAB.Core.Services.Models;

using Connection = openHAB.Core.Client.Connection.Models.Connection;

namespace openHAB.Core.Services;

/// <inheritdoc/>
public class IconCaching : IIconCaching
{
    private readonly IConnectionService _connectionService;
    private readonly IAppManager _appManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<ConnectionOptions> _connectionOptions;
    private readonly ILogger<IconCaching> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IconCaching" /> class.
    /// </summary>
    /// <param name="appPaths">Application default paths.</param>
    /// <param name="connectionService">The connection service to retrieve the connection details.</param>
    /// <param name="connectionOptions">The settings service to load settings.</param>
    /// <param name="logger">The logger.</param>
    public IconCaching(
        IConnectionService connectionService,
        IAppManager appManager,
        IHttpClientFactory httpClientFactory,
        IOptions<ConnectionOptions> connectionOptions,
        ILogger<IconCaching> logger)
    {
        _logger = logger;
        _connectionService = connectionService;
        _connectionOptions = connectionOptions;
        _appManager = appManager;
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task<string> ResolveIconPath(string icon, string state, string iconFormat)
    {
        string serverUrl = _connectionService.CurrentConnection.Url;
        OpenHABVersion openHABVersion = _appManager.ServerVersion;

        string iconUrl = openHABVersion >= OpenHABVersion.Two ?
                   $"{serverUrl}icon/{icon}?state={state}&format={iconFormat}&anyFormat=true&iconset=classic" :
                   $"{serverUrl}images/{icon}.png";

        try
        {
            Match iconName = Regex.Match(iconUrl, "icon/[0-9a-zA-Z]*", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
            Match iconState = Regex.Match(iconUrl, "state=[0-9a-zA-Z=]*", RegexOptions.None, TimeSpan.FromMilliseconds(1000));

            if (!iconName.Success)
            {
                throw new ServiceException("Can not resolve icon name from url");
            }

            if (!iconState.Success)
            {
                throw new ServiceException("Can not resolve icon state from url");
            }

            DirectoryInfo iconDirectory = EnsureIconCacheFolder();

            string iconFileName = $"{iconName.Value.Replace("icon/", string.Empty)}.{iconFormat}";
            string iconFilePath = Path.Combine(iconDirectory.FullName, iconFileName).Replace("NULL", string.Empty);

            if (File.Exists(iconFilePath))
            {
                return iconFilePath;
            }

            bool downloadSuccessfull = await DownloadAndSaveIconToCache(iconUrl, iconFilePath);

            return downloadSuccessfull ? iconFilePath : iconUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache icon");
            return iconUrl;
        }
    }

    private async Task<bool> DownloadAndSaveIconToCache(string iconUrl, string iconFilePath)
    {
        ConnectionOptions settings = _connectionOptions.Value;
        bool isRunningInDemoMode = settings.IsRunningInDemoMode.HasValue && settings.IsRunningInDemoMode.Value;
        Connection connection = await _connectionService.DetectAndRetrieveConnection(settings.LocalConnection, settings.RemoteConnection, isRunningInDemoMode)
            .ConfigureAwait(false);

        if (connection == null)
        {
            _logger.LogError("Failed to retrieve connection details to download icon");
            return false;
        }

        HttpClient httpClient = connection.Type switch
        {
            HttpClientType.Local => _httpClientFactory.CreateClient("local"),
            HttpClientType.Remote => _httpClientFactory.CreateClient("remote"),
            _ => throw new OpenHABException("Invalid connection type")
        };

        Uri uri = new Uri(iconUrl);
        HttpResponseMessage httpResponse = await httpClient.GetAsync(uri).ConfigureAwait(false);
        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to download icon from '{IconUrl}' with status code '{StatusCode}'", iconUrl, httpResponse.StatusCode);
        }

        byte[] iconContent = await httpResponse.Content.ReadAsByteArrayAsync();
        using (FileStream file = File.Open(iconFilePath, FileMode.OpenOrCreate))
        {
            await file.WriteAsync(iconContent, 0, iconContent.Length);
        }

        return true;
    }

    /// <inheritdoc/>
    public void ClearIconCache()
    {
        DirectoryInfo iconDirectory = EnsureIconCacheFolder();
        if (iconDirectory.Exists)
        {
            iconDirectory.Delete(true);
        }
    }

    private DirectoryInfo EnsureIconCacheFolder()
    {
        if (!Directory.Exists(AppPaths.IconCacheDirectory))
        {
            DirectoryInfo directory = Directory.CreateDirectory(AppPaths.IconCacheDirectory);
            _logger.LogInformation("Created icon cache directory '{Directory}'", directory.FullName);
            return directory;
        }

        return new DirectoryInfo(AppPaths.IconCacheDirectory);
    }
}
