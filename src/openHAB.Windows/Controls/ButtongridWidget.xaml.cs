using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using openHAB.Core.Client.Messages;
using openHAB.Core.Client.Models;

namespace openHAB.Windows.Controls;

/// <summary>
/// Widget control that represents an OpenHAB Buttongrid: mapping buttons placed on a grid.
/// </summary>
public sealed partial class ButtongridWidget : WidgetBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ButtongridWidget"/> class.
    /// </summary>
    public ButtongridWidget()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        BuildGrid();
    }

    private void BuildGrid()
    {
        ButtonGrid.Children.Clear();
        ButtonGrid.RowDefinitions.Clear();
        ButtonGrid.ColumnDefinitions.Clear();

        ICollection<WidgetMapping> mappings = Widget?.Mappings;
        if (mappings == null || mappings.Count == 0)
        {
            return;
        }

        int rows = mappings.Max(m => m.Row);
        int columns = mappings.Max(m => m.Column);

        for (int r = 0; r < rows; r++)
        {
            ButtonGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (int c = 0; c < columns; c++)
        {
            ButtonGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        foreach (WidgetMapping mapping in mappings)
        {
            Button button = new Button
            {
                Content = mapping.Label,
                Tag = mapping.Command,
                Margin = new Thickness(2),
                MinWidth = 48,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            button.Click += Button_Click;

            Grid.SetRow(button, System.Math.Max(0, mapping.Row - 1));
            Grid.SetColumn(button, System.Math.Max(0, mapping.Column - 1));
            ButtonGrid.Children.Add(button);
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (Widget?.Item == null || sender is not Button button)
        {
            return;
        }

        // ponytail: sends mapping.command on click; per-button releaseCommand/momentary behavior
        // deferred until a server actually populates it (needs PointerPressed/Released handling).
        StrongReferenceMessenger.Default.Send(new TriggerCommandMessage(Widget.Item, button.Tag?.ToString()));
    }

    internal override void SetState()
    {
    }
}
