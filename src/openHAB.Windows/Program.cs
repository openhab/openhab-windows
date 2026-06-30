using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
        // Initialize WinUI 3 first, before building the host
        XamlCheckProcessRequirements();

        WinRT.ComWrappersSupport.InitializeComWrappers();

        // Build the Generic Host before starting the XAML application. Building it
        // inside the App constructor runs it within the native WinUI Application
        // factory callback, where any failure is rethrown across the native boundary
        // and surfaces as an opaque COMException 0x8000FFFF (E_UNEXPECTED).
        InitializeHost();

        Application.Start((p) =>
        {
            var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
            System.Threading.SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }

    private static void InitializeHost()
    {
        // Ensure directories exist
        EnsureDirectoriesExist();

        var builder = new HostApplicationBuilder();
        builder.Configuration.AddJsonFile(AppPaths.SettingsFilePath, optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile(AppPaths.ConnectionFilePath, optional: true, reloadOnChange: true);

        builder.Services.AddConfiguration(builder.Configuration);
        builder.Services.AddOpenHABServices();
        builder.Services.AddOpenHABViewModels();
        builder.Services.AddViews();

        Host = builder.Build();
    }

    private static void EnsureDirectoriesExist()
    {
        try
        {
            if (!Directory.Exists(AppPaths.ApplicationDataDirectory))
            {
                Directory.CreateDirectory(AppPaths.ApplicationDataDirectory);
            }

            if (!Directory.Exists(AppPaths.LogsDirectory))
            {
                Directory.CreateDirectory(AppPaths.LogsDirectory);
            }

            if (!Directory.Exists(AppPaths.IconCacheDirectory))
            {
                Directory.CreateDirectory(AppPaths.IconCacheDirectory);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to create directories: {ex.Message}");
            // Don't throw here, let the app continue
        }
    }
}

#endif
