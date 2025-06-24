using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using openHAB.Core;

namespace openHAB.Windows;

#if DISABLE_XAML_GENERATED_MAIN
public static partial class Program
{
    public static IHost Host
    {
        get; set;
    }

    /// <summary>
    /// Ensures that the process can run XAML, and provides a deterministic
    /// error if a check fails. Otherwise, it quietly does nothing.
    /// </summary>
    [LibraryImport("Microsoft.ui.xaml.dll")]
    private static partial void XamlCheckProcessRequirements();

    [STAThread]
    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = new HostApplicationBuilder(args);
        builder.Configuration.AddJsonFile(AppPaths.SettingsFilePath, optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile(AppPaths.ConnectionFilePath, optional: true, reloadOnChange: true);

        builder.Services.AddConfiguration(builder.Configuration);
        builder.Services.AddOpenHABServices();
        builder.Services.AddOpenHABViewModels();
        builder.Services.AddViews();

        Host = builder.Build();

        // Taken from the default generated XAML entry point
        XamlCheckProcessRequirements();
        WinRT.ComWrappersSupport.InitializeComWrappers();

        Application.Start(_ =>
        {
            try
            {
                DispatcherQueue queue = DispatcherQueue.GetForCurrentThread();
                if (queue == null)
                {
                    throw new InvalidOperationException("Failed to get DispatcherQueue for the current thread.");
                }

                DispatcherQueueSynchronizationContext? context = new DispatcherQueueSynchronizationContext(queue);
                if (context == null)
                {
                    throw new InvalidOperationException("Failed to create DispatcherQueueSynchronizationContext.");
                }

                SynchronizationContext.SetSynchronizationContext(context);

                App? app = Host.Services.GetRequiredService<App>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in application start callback: {ex.Message}.");
            }
        });
    }
}

#endif
