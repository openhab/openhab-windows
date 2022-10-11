using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using openHAB.Core.Common;
using openHAB.Core.Model;
using openHAB.Core.Notification;
using openHAB.Core.Notification.Contracts;
using openHAB.Core.SDK;
using openHAB.Core.Services;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.ViewModel;
using Windows.Storage;

namespace openHAB.Windows.Services
{
    /// <summary>
    /// Dependency Injection Service.
    /// </summary>
    public class DIService : IDependencyInjectionService
    {
        private static DIService _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIService"/> class.
        /// </summary>
        public DIService()
        {
            _host = Host.CreateDefaultBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            RegisterServices(services);
                            RegisterViewModels(services);
                        })
                        .Build();
        }

        private readonly IHost _host;

        private void RegisterServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
             {
                 // configure Logging with NLog
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                 loggingBuilder.AddNLog(GetLoggingConfiguration());
             });

            services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
            services.AddSingleton<IOpenHAB, OpenHABClient>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddTransient<Settings>(x =>
            {
                ISettingsService settingsService = x.GetService<ISettingsService>();
                return settingsService.Load();
            });

            //_services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<OpenHABHttpClient>();
            services.AddSingleton<IConnectionService, ConnectionService>();
            services.AddSingleton<IIconCaching, IconCaching>();
            services.AddSingleton<IAppManager, AppManager>();
            services.AddSingleton<IItemManager, ItemManager>();
            services.AddSingleton<INotificationManager, NotificationManager>();
            services.AddSingleton<IOpenHABEventParser, OpenHABEventParser>();
        }

        private void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MainViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<ConfigurationViewModel>();
            services.AddTransient<LogsViewModel>();
        }

        private LoggingConfiguration GetLoggingConfiguration()
        {
            JsonLayout layout = new JsonLayout()
            {
                IncludeEventProperties = true,

            };

            layout.Attributes.Add(new JsonAttribute("time", @"${date:format=HH\:mm\:ss}"));
            layout.Attributes.Add(new JsonAttribute("level", "${level:upperCase=true}"));
            layout.Attributes.Add(new JsonAttribute("callsite", "${callsite:includeSourcePath=false}"));
            layout.Attributes.Add(new JsonAttribute("message", "${message}"));
            layout.Attributes.Add(new JsonAttribute("exception", "${exception:format=ToString}"));
            layout.Attributes.Add(new JsonAttribute("stacktrace", "${onexception:inner=${stacktrace:topFrames=10}}"));

            FileTarget fileTarget = new FileTarget("file")
            {
                FileName = "${var:LogPath}/logs/${shortdate}.log",
                Layout = layout,
                MaxArchiveFiles = 3,
                ArchiveEvery = FileArchivePeriod.Day,
            };

            LoggingConfiguration configuration = new LoggingConfiguration();
            configuration.AddTarget(fileTarget);
            configuration.AddRuleForAllLevels(fileTarget);

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            configuration.Variables["LogPath"] = storageFolder.Path;

            return configuration;
        }

        /// <summary>
        /// Gets the DIService instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DIService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DIService();
                }

                return _instance;
            }
        }

        /// <inheritdoc/>
        public T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }
    }
}
