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
    public class SitemapService
    {
        private readonly ISettingsService _settingsService;
        private readonly IOpenHABClient _openHABClient;
        private readonly ILogger<SitemapService> _logger;
        private ServerInfo _serverInfo;

        public SitemapService(ISettingsService settingsService, IOpenHABClient openHABClient, ILogger<SitemapService> logger)
        {
            _settingsService = settingsService;
            _openHABClient = openHABClient;
            _logger = logger;
        }

        public async Task<List<OpenHABSitemap>> GetSitemaps(CancellationToken loadCancellationToken)
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

                Func<OpenHABSitemap, bool> defaultSitemapFilter = (sitemap) =>
                {
                    return !sitemap.Name.Equals("_default", StringComparison.InvariantCultureIgnoreCase);
                };

                Settings settings = _settingsService.Load();
                List<Func<OpenHABSitemap, bool>> filters = new List<Func<OpenHABSitemap, bool>>();
                if (!settings.ShowDefaultSitemap)
                {
                    filters.Add(defaultSitemapFilter);
                }

                ICollection<OpenHABSitemap> sitemaps = await _openHABClient.LoadSitemaps(_serverInfo.Version, filters).ConfigureAwait(false);
                return new List<OpenHABSitemap>(sitemaps);
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

        public async Task<ICollection<OpenHABWidget>> LoadItemsFromSitemapAsync(OpenHABSitemap model)
        {
            ICollection<OpenHABWidget> widgetModels = await _openHABClient.LoadItemsFromSitemap(model.Link, _serverInfo.Version).ConfigureAwait(false);
            return widgetModels;
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

        public async Task<HttpResponseResult<bool>> SendItemCommand(OpenHABItem item, string command)
        {
            HttpResponseResult<bool> result = await _openHABClient.SendCommand(item, command).ConfigureAwait(false);
            return result;
        }
    }
}
