using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

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

        Application.Start((p) =>
        {
            var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
            System.Threading.SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }
}

#endif
