using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using openHAB.Core.Common;
using openHAB.Core.Connection;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Services
{
    /// <inheritdoc/>
    public class IconCaching : IIconCaching
    {
        private readonly string _iconCacheDirectory = "icons";
        private OpenHABHttpClient _openHABHttpClient;
        private IConnectionService _connectionService;
        private Settings _settings;
        private AppPaths _applicationContext;
        private ILogger<IconCaching> _logger;

        /// <summary>Initializes a new instance of the <see cref="IconCaching" /> class.</summary>
        /// <param name="openHABHttpClient">HTTP Client factory.</param>
        /// <param name="connectionService">ConnectionService to retrive the connection details.</param>
        /// <param name="settingsService">Setting Service to load settings.</param>
        /// <param name="logger">The logger.</param>
        public IconCaching(
            AppPaths applicationContext,
            OpenHABHttpClient openHABHttpClient,
            IConnectionService connectionService,
            ISettingsService settingsService,
            ILogger<IconCaching> logger)
        {
            _logger = logger;
            _openHABHttpClient = openHABHttpClient;
            _connectionService = connectionService;
            _settings = settingsService.Load();

            _applicationContext = applicationContext;
        }

        /// <inheritdoc/>
        public async Task<string> ResolveIconPath(string iconUrl, string iconFormat)
        {
            try
            {
                Match iconName = Regex.Match(iconUrl, "icon/[0-9a-zA-Z]*", RegexOptions.None, TimeSpan.FromMilliseconds(100));
                Match iconState = Regex.Match(iconUrl, "state=[0-9a-zA-Z=]*", RegexOptions.None, TimeSpan.FromMilliseconds(100));

                if (!iconName.Success)
                {
                    throw new OpenHABException("Can not resolve icon name from url");
                }

                if (!iconState.Success)
                {
                    throw new OpenHABException("Can not resolve icon state from url");
                }

                if (!iconState.Success)
                {
                    throw new OpenHABException("Can not resolve icon state from url");
                }

                DirectoryInfo iconDirectory = EnsureIconCacheFolder();

                string iconFileName = $"{iconName.Value.Replace("icon/", string.Empty)}{iconState.Value.Replace("state=", string.Empty)}.{iconFormat}";
                string iconFilePath = Path.Combine(iconDirectory.FullName, iconFileName).Replace("NULL", string.Empty);

                if (File.Exists(iconFilePath))
                {
                    return iconFilePath;
                }

                await DownloadAndSaveIconToCache(iconUrl, iconFilePath);
                return iconFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache icon");
                return iconUrl;
            }
        }

        private async Task DownloadAndSaveIconToCache(string iconUrl, string iconFilePath)
        {
            OpenHABConnection connection = await _connectionService.DetectAndRetriveConnection(_settings).ConfigureAwait(false);
            using (HttpClient httpClient = _openHABHttpClient.DisposableClient(connection, _settings))
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(new Uri(iconUrl)).ConfigureAwait(false);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to download icon from '{iconUrl}' with status code '{httpResponse.StatusCode}'");
                    return;
                }

                byte[] iconContent = await httpResponse.Content.ReadAsByteArrayAsync();

                using (FileStream file = File.Create(iconFilePath))
                {
                    await file.WriteAsync(iconContent, 0, iconContent.Length);
                }
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
