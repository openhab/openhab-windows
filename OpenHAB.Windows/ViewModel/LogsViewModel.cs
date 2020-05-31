using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OpenHAB.Windows.ViewModel
{
    /// <summary>
    /// ViewModel for application logs.
    /// </summary>
    /// <seealso cref="ViewModelBase{System.Object}" />
    public class LogsViewModel : ViewModelBase<object>
    {
        private StorageFile _logFile;
        private string _logFileContent;
        private FileSystemWatcher _logFileWatcher;
        private ILogger<LogsViewModel> _logger;

        /// <summary>Initializes a new instance of the <see cref="LogsViewModel" /> class.</summary>
        public LogsViewModel(ILogger<LogsViewModel> logger)
            : base(null)
        {
            _logger = logger;
            LoadLogfileContentAsync();
        }

        /// <summary>Gets or sets the content of the log.</summary>
        /// <value>The content of the log.</value>
        public string LogContent
        {
            get
            {
                return _logFileContent;
            }

            set
            {
                Set(ref _logFileContent, value);
            }
        }

        /// <summary>Gets the log file path.</summary>
        /// <value>The log file path.</value>
        public string LogFilePath
        {
            get
            {
                if (_logFile != null)
                {
                    return _logFile.Path;
                }

                return string.Empty;
            }
        }

        private async Task LoadLogfileContentAsync()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFolder logFolder = await storageFolder.GetFolderAsync("logs");
            if (logFolder == null)
            {
                return;
            }

            _logFile = await logFolder.GetFileAsync($"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            if (_logFile == null)
            {
                return;
            }

            LogContent = await FileIO.ReadTextAsync(_logFile);

            //_logFileWatcher = new FileSystemWatcher();
            //_logFileWatcher.Path = _logFile.Path;
            //_logFileWatcher.Changed += LogFile_Changed;
        }

        private async void LogFile_Changed(object sender, FileSystemEventArgs e)
        {
            LogContent = await FileIO.ReadTextAsync(_logFile);
        }
    }
}
