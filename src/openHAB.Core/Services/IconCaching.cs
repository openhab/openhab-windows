using CommunityToolkit.WinUI.Helpers;
using Microsoft.Extensions.Logging;
using openHAB.Core.Common;
using openHAB.Core.Connection;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace openHAB.Core.Services
{
    /// <inheritdoc/>
    public class IconCaching : IIconCaching
    {
        private string _iconCacheDirectory = "icons";
        private StorageFolder _cacheFolder;
        private OpenHABHttpClient _openHABHttpClient;
        private IConnectionService _connectionService;
        private ISettingsService _settingsService;
        private Settings _settings;
        private ILogger<IconCaching> _logger;

        /// <summary>Initializes a new instance of the <see cref="IconCaching" /> class.</summary>
        /// <param name="openHABHttpClient">HTTP Client factory.</param>
        /// <param name="connectionService">ConnectionService to retrive the connection details.</param>
        /// <param name="settingsService">Setting Service to load settings.</param>
        /// <param name="logger">The logger.</param>
        public IconCaching(OpenHABHttpClient openHABHttpClient, IConnectionService connectionService,
            ISettingsService settingsService, ILogger<IconCaching> logger)
        {
            _logger = logger;
            _cacheFolder = ApplicationData.Current.LocalCacheFolder;
            _openHABHttpClient = openHABHttpClient;
            _connectionService = connectionService;
            _settingsService = settingsService;
            _settings = settingsService.Load();
        }

        /// <inheritdoc/>
        public async Task<string> ResolveIconPath(string iconUrl, string iconFormat)
        {
            try
            {
                Match iconName = Regex.Match(iconUrl, "icon/[0-9a-zA-Z]*");
                Match iconState = Regex.Match(iconUrl, "state=[0-9a-zA-Z=]*");

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

                StorageFolder storageFolder = await EnsureIconCacheFolder();

                string iconFileName = $"{iconName.Value.Replace("icon/", string.Empty)}{iconState.Value.Replace("state=", string.Empty)}.{iconFormat}";
                string iconFilePath = $"{storageFolder.Path}\\{iconFileName}";

                if (await storageFolder.FileExistsAsync(iconFileName))
                {
                    return iconFilePath;
                }

                await DownloadAndSaveIconToCache(iconUrl, iconFileName, storageFolder);
                return iconFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cache icon");
                return iconUrl;
            }
        }

        private async Task DownloadAndSaveIconToCache(string iconUrl, string iconFileName, StorageFolder storageFolder)
        {
            OpenHABConnection connection = await _connectionService.DetectAndRetriveConnection(_settings).ConfigureAwait(false);
            using (HttpClient httpClient = _openHABHttpClient.DisposableClient(connection, _settings))
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(new Uri(iconUrl)).ConfigureAwait(false);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return;
                }

                byte[] iconContent = await httpResponse.Content.ReadAsByteArrayAsync();
                StorageFile file = await storageFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(file, iconContent);
            }
        }

        /// <inheritdoc/>
        public async void ClearIconCache()
        {
            StorageFolder storageFolder = await EnsureIconCacheFolder();
            await storageFolder.DeleteAsync();
        }

        private async Task<StorageFolder> EnsureIconCacheFolder()
        {
            StorageFolder storageFolder = await _cacheFolder.TryGetItemAsync(_iconCacheDirectory) as StorageFolder;

            if (storageFolder == null)
            {
                storageFolder = await _cacheFolder.CreateFolderAsync(_iconCacheDirectory);
            }

            return storageFolder;
        }
    }
}
