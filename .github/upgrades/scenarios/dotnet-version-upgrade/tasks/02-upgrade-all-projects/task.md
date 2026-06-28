# 02-upgrade-all-projects: Upgrade all projects to .NET 10

Update all 6 projects to target .NET 10, update package references, and resolve API compatibility issues. This is the core atomic upgrade covering:

**Projects** (topological order):
- openHAB.Common (shared resources)
- openHAB.Core.Client (REST API client, SSE events)
- openHAB.Core (application services)
- openHAB.Windows (WinUI 3 app - Views, ViewModels, controls)
- openHAB.Windows.Package + .Beta (MSIX packaging)

**Package Issues**:
- 2 incompatible packages requiring inline resolution:
  - Mapsui.WinUI (5.1.0) — investigate .NET 10 compatible version or alternative
  - Microsoft.Xaml.Behaviors.WinUI.Managed (3.0.1) — investigate replacement
- 20 compatible packages to update to latest .NET 10-compatible versions
- System.Text.RegularExpressions (in Package projects) — already included in framework reference

**API Issues** (261 total, resolve inline):
- **Windows.UI.Color** (63 occurrences) — source incompatible, migrate to Windows.UI namespace changes
- **System.Uri** (51 occurrences) — behavioral changes in .NET 10
- **Windows.Foundation.Point** (31 occurrences) — source incompatible, type system changes
- **Windows.Foundation.Size, Rect** (multiple occurrences) — source incompatible
- **TimeSpan.FromMilliseconds** overload changes (5 occurrences)
- 2 binary incompatible APIs requiring code changes
- 158 source incompatible APIs needing recompilation and potential conflict resolution
- 101 behavioral changes requiring careful review

**Research starting points**:
- Check WinUI 3 migration guide for Windows.UI.Color → new color APIs
- Investigate Windows.Foundation geometry type changes
- Review System.Uri behavioral changes in .NET 10
- Search for Mapsui.WinUI alternatives or updated versions
- Check CommunityToolkit alternatives for XAML behaviors

**Done when**: All projects target net10.0-windows10.0.26100.0, all packages updated or replaced, solution builds with 0 errors and 0 warnings, all 261 API issues resolved.
