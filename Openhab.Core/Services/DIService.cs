using System;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Layouts;
using NLog.Targets;
using OpenHAB.Core.Contracts.Services;
using OpenHAB.Core.Model;
using OpenHAB.Core.SDK;
using OpenHAB.Core.ViewModel;
using Windows.Storage;

namespace OpenHAB.Core.Services
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
                 loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                 loggingBuilder.AddNLog(GetLoggingConfiguration());
             });

            _services.AddSingleton(Messenger.Default);
            _services.AddSingleton<IOpenHAB, SDK.OpenHABClient>();
            _services.AddSingleton<ISettingsService, SettingsService>();
            _services.AddSingleton<INavigationService, NavigationService>();
        }

        private void RegisterViewModels()
        {
            _services.AddTransient<MainViewModel>();
            _services.AddTransient<SettingsViewModel>();
            _services.AddTransient<ConfigurationViewModel>();
        }

        private LoggingConfiguration GetLoggingConfiguration()
        {
            CsvLayout layout = new CsvLayout()
            {
                Delimiter = CsvColumnDelimiterMode.Semicolon
            };

            layout.Columns.Add(new CsvColumn("time", @"${date:format=HH\:mm\:ss}"));
            layout.Columns.Add(new CsvColumn("level", "${level:upperCase=true}"));
            layout.Columns.Add(new CsvColumn("message", "${message}"));
            layout.Columns.Add(new CsvColumn("callsite", "${callsite:includeSourcePath=false}"));
            layout.Columns.Add(new CsvColumn("exception", "${exception:format=ToString}"));
            layout.Columns.Add(new CsvColumn("stacktrace", "${onexception:inner=${stacktrace:topFrames=10}}"));

            FileTarget fileTarget = new FileTarget("file")
            {
                FileName = "${var:LogPath}/logs/${shortdate}.log",
                Layout = layout,
                MaxArchiveFiles = 3,
                ArchiveEvery = FileArchivePeriod.Day
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
