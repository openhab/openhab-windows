using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using OpenHAB.Core.Common;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.Services;
using OpenHAB.Windows.ViewModel;
using Windows.Storage;

namespace OpenHAB.Windows.Services
{
    /// <summary>
    /// Dependency Injection Service.
    /// </summary>
    public class DIService : IDependencyInjectionService
    {
        private static DIService _instance;
        private readonly ServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="DIService"/> class.
        /// </summary>
        public DIService()
        {
            _services = new ServiceCollection();
            RegisterServices();
            RegisterViewModels();

            Services = _services.BuildServiceProvider();
        }

        private void RegisterServices()
        {
            _services.AddLogging(loggingBuilder =>
             {
                 // configure Logging with NLog
                 loggingBuilder.ClearProviders();
                 loggingBuilder.SetMinimumLevel(LogLevel.Information);
                 loggingBuilder.AddNLog(GetLoggingConfiguration());
             });

            _services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
            _services.AddSingleton<IOpenHAB, OpenHABClient>();
            _services.AddSingleton<ISettingsService, SettingsService>();
            _services.AddTransient<Settings>(x =>
            {
                ISettingsService settingsService = x.GetService<ISettingsService>();
                return settingsService.Load();
            });

            //_services.AddSingleton<INavigationService, NavigationService>();
            _services.AddSingleton<OpenHABHttpClient>();
            _services.AddSingleton<IIconCaching, IconCaching>();
            _services.AddSingleton<IAppManager, AppManager>();
            _services.AddSingleton<IItemManager, ItemManager>();
            _services.AddSingleton<INotificationManager, NotificationManager>();
            _services.AddSingleton<IOpenHABEventParser, OpenHABEventParser>();
        }

        private void RegisterViewModels()
        {
            _services.AddTransient<MainViewModel>();
            _services.AddTransient<SettingsViewModel>();
            _services.AddTransient<ConfigurationViewModel>();
            _services.AddTransient<LogsViewModel>();
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
                FileName = "${var:LogPath}/logs/${shortdate}.json",
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

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public ServiceProvider Services
        {
            get;
            private set;
        }
    }
}
