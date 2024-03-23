using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using openHAB.Core.Client.Common;
using openHAB.Core.Client.Contracts;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;
using openHAB.Core.Messages;
using openHAB.Core.Model;
using openHAB.Core.Services.Contracts;

namespace openHAB.Core.Services
{
    /// <summary>
    /// Service for managing sitemaps in openHAB.
    /// </summary>
    public class SitemapService
    {
        private readonly ILogger<SitemapService> _logger;
        private readonly IOpenHABClient _openHABClient;
        private readonly ISettingsService _settingsService;
        private ServerInfo _serverInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitemapService"/> class.
        /// </summary>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="openHABClient">The openHAB client.</param>
        /// <param name="logger">The logger.</param>
        public SitemapService(ISettingsService settingsService, IOpenHABClient openHABClient, ILogger<SitemapService> logger)
        {
            _settingsService = settingsService;
            _openHABClient = openHABClient;
            _logger = logger;
        }

        /// <summary>
        /// Gets the sitemap by URL.
        /// </summary>
        /// <param name="sitemapUrl">The sitemap URL.</param>
        /// <returns>The <see cref="Sitemap"/> object representing the sitemap.</returns>
        public async Task<Sitemap> GetSitemapByUrlAsync(string sitemapUrl)
        {
            try
            {
                _serverInfo = await InitalizeConnectionAsync();
                if (_serverInfo == null)
                {
                    return null;
                }
                _settingsService.ServerVersion = _serverInfo.Version;

                Sitemap sitemap = await _openHABClient.GetSitemap(sitemapUrl, _serverInfo.Version).ConfigureAwait(false);
                return sitemap;
            }
            catch (OpenHABException ex)
            {
                _logger.LogError(ex, $"Loading sitemap {sitemapUrl} failed.");
                StrongReferenceMessenger.Default.Send(new ConnectionErrorMessage(ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load sitemap data failed.");
            }

            return null;
        }

        /// <summary>
        /// Gets the list of sitemaps.
        /// </summary>
        /// <param name="loadCancellationToken">The cancellation token for the load operation.</param>
        /// <returns>The list of <see cref="Sitemap"/> objects representing the sitemaps.</returns>
        public async Task<List<Sitemap>> GetSitemapsAsync(CancellationToken loadCancellationToken)
        {
            try
            {
                _serverInfo = await InitalizeConnectionAsync();
                if (_serverInfo == null)
                {
                    return null;
                }
                _settingsService.ServerVersion = _serverInfo.Version;

                if (loadCancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                Func<Sitemap, bool> defaultSitemapFilter = (sitemap) =>
                {
                    return !sitemap.Name.Equals("_default", StringComparison.InvariantCultureIgnoreCase);
                };

                Settings settings = _settingsService.Load();
                List<Func<Sitemap, bool>> filters = new List<Func<Sitemap, bool>>();
                if (!settings.ShowDefaultSitemap)
                {
                    filters.Add(defaultSitemapFilter);
                }

                ICollection<Sitemap> sitemaps = await _openHABClient.LoadSitemaps(_serverInfo.Version, filters).ConfigureAwait(false);
                return new List<Sitemap>(sitemaps);
            }
            catch (OpenHABException ex)
            {
                _logger.LogError(ex, "Loading sitemaps failed.");
                StrongReferenceMessenger.Default.Send(new ConnectionErrorMessage(ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load data failed.");
            }

            return null;
        }

        /// <summary>
        /// Loads the items from a sitemap.
        /// </summary>
        /// <param name="model">The sitemap model.</param>
        /// <returns>The collection of <see cref="Widget"/> objects representing the items.</returns>
        public async Task<ICollection<Widget>> LoadItemsFromSitemapAsync(Sitemap model)
        {
            ICollection<Widget> widgetModels = await _openHABClient.LoadItemsFromSitemap(model.Link, _serverInfo.Version).ConfigureAwait(false);
            return widgetModels;
        }

        /// <summary>
        /// Sends a command to an item.
        /// </summary>
        /// <param name="item">The item to send the command to.</param>
        /// <param name="command">The command to send.</param>
        /// <returns>The <see cref="HttpResponseResult{T}"/> object representing the result of the command.</returns>
        public async Task<HttpResponseResult<bool>> SendItemCommand(Item item, string command)
        {
            HttpResponseResult<bool> result = await _openHABClient.SendCommand(item, command).ConfigureAwait(false);
            return result;
        }

        private async Task<ServerInfo> InitalizeConnectionAsync()
        {
            Settings settings = _settingsService.Load();
            if (settings.LocalConnection == null && settings.RemoteConnection == null &&
                (!settings.IsRunningInDemoMode.HasValue || !settings.IsRunningInDemoMode.Value))
            {
                StrongReferenceMessenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                return null;
            }

            bool isSuccessful = await _openHABClient.ResetConnection(settings.LocalConnection, settings.RemoteConnection, settings.IsRunningInDemoMode)
                .ConfigureAwait(false);
            if (!isSuccessful)
            {
                StrongReferenceMessenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                return null;
            }

            HttpResponseResult<ServerInfo> result = await _openHABClient.GetOpenHABServerInfo().ConfigureAwait(false);
            ServerInfo serverInfo = result?.Content;

            if (serverInfo == null || serverInfo.Version == OpenHABVersion.None)
            {
                StrongReferenceMessenger.Default.Send(new FireInfoMessage(MessageType.NotConfigured));
                return null;
            }

            return serverInfo;
        }
    }
}
