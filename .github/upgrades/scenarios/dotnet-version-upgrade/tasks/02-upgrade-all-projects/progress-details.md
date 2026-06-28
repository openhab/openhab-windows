# Upgrade All Projects to .NET 10 — Task 02 Progress

## What Was Done

Upgraded all 6 projects from .NET 9 to .NET 10 and resolved all breaking API changes.

## Files Modified

### Project Files (TFM updates)
- `src/openHAB.Common/openHAB.Common.csproj` — `net9.0-windows10.0.26100.0` → `net10.0-windows10.0.26100.0`
- `src/openHAB.Core.Client/openHAB.Core.Client.csproj` — TFM → net10.0
- `src/openHAB.Core/Openhab.Core.csproj` — TFM → net10.0
- `src/openHAB.Windows/openHAB.Windows.csproj` — TFM → net10.0; removed redundant `System.Text.Json` package reference (NU1510)

> Note: The two MSIX packaging projects (`openHAB.Windows.Package.wapproj`, `openHAB.Windows.Package.Beta.wapproj`) use UAP `TargetPlatformVersion` (10.0.26100.0), not a .NET TFM — no TFM change required. They reference the main app project which is now on .NET 10.

### Code Files (API breaking changes)
- `src/openHAB.Windows/Controls/MapViewWidget.xaml.cs`:
  - Mapsui 5.x: `ZoomInOutWidget` moved from namespace `Mapsui.Widgets.Zoom` → `Mapsui.Widgets.ButtonWidgets`
  - Mapsui 5.x: `Map.Home` (an `Action<Navigator>`) was removed. Replaced with direct `Map.Navigator.CenterOnAndZoomTo(coordinate, Map.Navigator.Viewport.Resolution)` calls.
  - Removed dead/unused local `Viewport viewport` variable.

## Package Resolution (Resolve Inline)

Two packages were flagged "incompatible" in the assessment because they lack a `net10.0-windows` asset:
- **Mapsui.WinUI 5.1.0** — latest version; ships net8.0/net9.0-windows builds. Resolves via net9.0 fallback, which is runtime-compatible with .NET 10. No newer version available.
- **Microsoft.Xaml.Behaviors.WinUI.Managed 3.0.1** — latest version; same situation, net9.0 fallback works.

Verified `get_supported_package_version` returns the same versions (5.1.0 / 3.0.1) — no .NET 10-specific releases exist yet. The net9.0 builds load cleanly under the net10.0 target (no NU1701 on the main project). No replacement needed.

## Build Result

- **openHAB.Windows.csproj** (builds Common, Core.Client, Core as dependencies): **Build succeeded, 0 errors**
- Configuration: Debug | x64

## Warnings

- **NU1510 (upgrade-introduced)**: FIXED — removed redundant `System.Text.Json` reference (now in .NET 10 shared framework).
- **~108 pre-existing warnings** (CS86xx nullable, SAxxxx StyleCop, CS4014 async): DEFERRED per user decision (Option 1). Confirmed via `git show HEAD` that `<Nullable>enable</Nullable>` and StyleCop were already configured before the upgrade — these warnings are not caused by the .NET 10 change. See scenario-instructions.md Key Decisions Log and Reminders.

## Issues Encountered & Resolved

1. **CS0234** `Mapsui.Widgets.Zoom` namespace not found → identified via assembly reflection that `ZoomInOutWidget` is now in `Mapsui.Widgets.ButtonWidgets`. Fixed the `using`.
2. **CS1061** `Map.Home` not defined → Mapsui 5.x removed the `Home` delegate; migrated to direct `Navigator` calls.
3. **NU1510** redundant `System.Text.Json` → removed explicit package reference.

## Notes for Final Validation (Task 03)

- Full-solution MSBuild including MSIX packaging (wapproj) is very slow (>15 min) because it generates app packages for all platforms. The main app project build is the fast validation path.
- NU1701 warnings appear on the wapproj packaging projects for transitive packages (BruTile, Mapsui.*, SkiaSharp) restored against UAP — these are pre-existing and inherent to the packaging project model.
- Tests should be run in task 03.
