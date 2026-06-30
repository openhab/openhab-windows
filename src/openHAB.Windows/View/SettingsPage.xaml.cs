using System;
using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using openHAB.Core.Services.Contracts;
using openHAB.Windows.Controls;
using openHAB.Windows.ViewModel;

namespace openHAB.Windows.View;

/// <summary>
/// In this page users can set their connection to the OpenHAB server.
/// </summary>
public sealed partial class SettingsPage : Page
{
    private readonly IAppManager _appManager;
    private readonly ILogger<SettingsPage> _logger;
    private readonly SettingsViewModel _settingsViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();

        _settingsViewModel = Program.Host.Services.GetRequiredService<SettingsViewModel>();
        _logger = Program.Host.Services.GetRequiredService<ILogger<SettingsPage>>();
        _appManager = Program.Host.Services.GetRequiredService<IAppManager>();

        DataContext = _settingsViewModel;
    }

    #region Page Navigation

    /// <inheritdoc/>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        AppAutostartSwitch.Toggled += AppAutostartSwitch_Toggled;
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        AppAutostartSwitch.Toggled -= AppAutostartSwitch_Toggled;
    }

    #endregion

    /// <summary>
    /// Gets the datacontext, for use in compiled bindings.
    /// </summary>
    public SettingsViewModel Vm => DataContext as SettingsViewModel;

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainUIPage));
    }

    private static ConnectionDialog CreateConnectionDialog()
    {
        ConnectionDialog connectionDialog = new ConnectionDialog();
        connectionDialog.DefaultButton = ContentDialogButton.Primary;

        return connectionDialog;
    }

    private async void OpenLocalConnectionButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectionDialog connectionDialog = CreateConnectionDialog();
        connectionDialog.DataContext = Vm.Settings.LocalConnection;

        /*TODO: Remove workaround when fix available
        https://github.com/microsoft/microsoft-ui-xaml/issues/2504 */
        connectionDialog.XamlRoot = this.Content.XamlRoot;

        await connectionDialog.ShowAsync();
    }

    private async void OpenRemoteConnectionButton_Click(object sender, RoutedEventArgs e)
    {
        ConnectionDialog connectionDialog = CreateConnectionDialog();
        connectionDialog.DataContext = Vm.Settings.RemoteConnection;

        /*TODO: Remove workaround when fix available
        https://github.com/microsoft/microsoft-ui-xaml/issues/2504 */
        connectionDialog.XamlRoot = this.Content.XamlRoot;

        await connectionDialog.ShowAsync();
    }

    private async void AppAutostartSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        ToggleSwitch toggleSwitch = (ToggleSwitch)e.OriginalSource;

        bool toggleIsOn = toggleSwitch.IsOn;
        var autostartEnabled = await _appManager.IsStartupEnabled().ConfigureAwait(false);

        await App.DispatcherQueue.EnqueueAsync(async () =>
        {
            if (autostartEnabled != toggleIsOn)
            {
                await _appManager.ToggleAutostart();
            }

            _settingsViewModel.Settings.IsAppAutostartEnabled = toggleIsOn;
        });
    }
}
