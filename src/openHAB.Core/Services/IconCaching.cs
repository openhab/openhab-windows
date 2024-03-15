using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.Extensions.Logging;
using openHAB.Core.Client;
using openHAB.Core.Client.Connection.Contracts;
using openHAB.Core.Client.Connection.Models;
using openHAB.Core.Client.Models;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Services
{
    /// <inheritdoc/>
    public class IconCaching : IIconCaching
    {
        private readonly OpenHABHttpClient _openHABHttpClient;
        private readonly IConnectionService _connectionService;
        private readonly ISettingsService _settingsService;
        private readonly Settings _settings;
        private readonly AppPaths _applicationContext;
        private readonly ILogger<IconCaching> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconCaching" /> class.
        /// </summary>
        /// <param name="appPaths">Application default paths.</param>
        /// <param name="openHABHttpClient">The HTTP client factory.</param>
        /// <param name="connectionService">The connection service to retrieve the connection details.</param>
        /// <param name="settingsService">The settings service to load settings.</param>
        /// <param name="logger">The logger.</param>
        public IconCaching(
            AppPaths appPaths,
            OpenHABHttpClient openHABHttpClient,
            IConnectionService connectionService,
            ISettingsService settingsService,
            ILogger<IconCaching> logger)
        {
            _logger = logger;
            _openHABHttpClient = openHABHttpClient;
            _connectionService = connectionService;
            _settingsService = settingsService;
            _settings = settingsService.Load();

            _applicationContext = appPaths;
        }

        /// <inheritdoc/>
        public async Task<string> ResolveIconPath(string icon, string state, string iconFormat)
        {
            string serverUrl = _connectionService.CurrentConnection.Url;
            OpenHABVersion openHABVersion = _settingsService.ServerVersion;

            string iconUrl = openHABVersion == OpenHABVersion.Two || openHABVersion == OpenHABVersion.Three || openHABVersion == OpenHABVersion.Four ?
                       $"{serverUrl}icon/{icon}?state={state}&format={iconFormat}" :
                       $"{serverUrl}images/{icon}.png";

            try
            {
                Match iconName = Regex.Match(iconUrl, "icon/[0-9a-zA-Z]*", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                Match iconState = Regex.Match(iconUrl, "state=[0-9a-zA-Z=]*", RegexOptions.None, TimeSpan.FromMilliseconds(100));

                if (!iconName.Success)
                {
                    throw new ServiceException("Can not resolve icon name from url");
                }

                if (!iconState.Success)
                {
                    throw new ServiceException("Can not resolve icon state from url");
                }

                if (!iconState.Success)
                {
                    throw new ServiceException("Can not resolve icon state from url");
                }

                DirectoryInfo iconDirectory = EnsureIconCacheFolder();

                //string iconFileName = $"{iconName.Value.Replace("icon/", string.Empty)}{iconState.Value.Replace("state=", string.Empty)}.{iconFormat}";
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
            bool isRunningInDemoMode = _settings.IsRunningInDemoMode.HasValue && _settings.IsRunningInDemoMode.Value;
            Connection connection = await _connectionService.DetectAndRetriveConnection(_settings.LocalConnection, _settings.RemoteConnection, isRunningInDemoMode)
                .ConfigureAwait(false);
            if (connection == null)
            {
                _logger.LogError("Failed to retrieve connection details to download icon");
                return false;
            }

            using (HttpClient httpClient = _openHABHttpClient.DisposableClient(connection))
            {
                Uri uri = new Uri(iconUrl);
                HttpResponseMessage httpResponse = await httpClient.GetAsync(uri).ConfigureAwait(false);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to download icon from '{iconUrl}' with status code '{httpResponse.StatusCode}'");
                    return false;
                }

                byte[] iconContent = await httpResponse.Content.ReadAsByteArrayAsync();
                using (FileStream file = File.Create(iconFilePath))
                {
                    await file.WriteAsync(iconContent, 0, iconContent.Length);
                }

                return true;
            }
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
            if (!Directory.Exists(_applicationContext.IconCacheDirectory))
            {
                DirectoryInfo directory = Directory.CreateDirectory(_applicationContext.IconCacheDirectory);
                _logger.LogInformation($"Created icon cache directory '{directory.FullName}'");
                return directory;
            }

            return new DirectoryInfo(_applicationContext.IconCacheDirectory);
        }
    }
}
