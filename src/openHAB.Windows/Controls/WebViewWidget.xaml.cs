using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace openHAB.Windows.Controls;

/// <summary>
/// Widget control that represents an OpenHab WebView.
/// </summary>
public sealed partial class WebViewWidget : WidgetBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebViewWidget"/> class.
    /// </summary>
    public WebViewWidget()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetState();
    }

    private async void OnTapped(object sender, TappedRoutedEventArgs e)
    {
        // Navigate the popup WebView2 only now, right before the dialog is shown.
        // WebViewFull lives inside PopupDialog (a ContentDialog) and is NOT part of the
        // live visual tree until the dialog is opened. Setting Source while it is not
        // loaded forces CoreWebView2 initialization in an invalid state, which surfaces
        // asynchronously as winrt::hresult_error 0x8007139F (ERROR_INVALID_STATE).
        if (Uri.TryCreate(Widget?.Url, UriKind.Absolute, out Uri uri))
        {
            WebViewFull.Source = uri;
        }

        await PopupDialog.ShowAsync();
    }

    internal override void SetState()
    {
        // Only navigate the inline WebView here. Do NOT touch WebViewFull: it is hosted in
        // a ContentDialog that has not been opened yet, so its CoreWebView2 is not in a
        // valid state and navigating it throws ERROR_INVALID_STATE (0x8007139F).
        if (!Uri.TryCreate(Widget?.Url, UriKind.Absolute, out Uri uri))
        {
            return;
        }

        // The widget can be recycled while the sitemap refreshes; if the control has been
        // unloaded its CoreWebView2 is gone and navigating it would also throw
        // ERROR_INVALID_STATE. Guard against that race.
        if (WebView is null || !IsLoaded)
        {
            return;
        }

        try
        {
            WebView.Source = uri;
        }
        catch (Exception)
        {
            // The CoreWebView2 may have been torn down between the IsLoaded check and the
            // navigation. Swallow it; OnLoaded/SetState will navigate again once the
            // control is ready.
        }
    }
}
