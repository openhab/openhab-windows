using openHAB.Core.Model;

/// <summary>
/// Represents a navigation between two OpenHAB widgets.
/// </summary>
public class WigetNavigation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WigetNavigation"/> class.
    /// </summary>
    /// <param name="originWidget">The origin widget.</param>
    /// <param name="targetWidget">The target widget.</param>
    public WigetNavigation(OpenHABWidget originWidget, OpenHABWidget targetWidget)
    {
        OriginWidget = originWidget;
        TargetWidget = targetWidget;
    }

    /// <summary>
    /// Gets or sets the origin widget.
    /// </summary>
    public OpenHABWidget OriginWidget { get; set; }

    /// <summary>
    /// Gets or sets the target widget.
    /// </summary>
    public OpenHABWidget TargetWidget { get; set; }
}
