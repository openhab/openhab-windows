# .NET 10 Upgrade Plan

## Overview

**Target**: Upgrade 6 projects from .NET 9 to .NET 10 (LTS)
**Scope**: Medium-sized WinUI 3 solution, ~11.5k LOC

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 6 projects, all on .NET 9 (modern .NET), clear 3-tier dependency structure, moderate complexity (2 incompatible packages + 261 API issues mostly WinUI type compatibility). Straightforward TFM bump with known migration patterns.

## Tasks

### 01-prerequisites: Verify SDK and tooling compatibility

Validate that the .NET 10 SDK is installed and accessible, and check for any global.json constraints that could block the upgrade. This includes verifying Windows App SDK compatibility with .NET 10 and ensuring required Windows SDKs (10.0.20348.0 and 10.0.26100.0) are available.

The solution uses Windows App SDK 2.2.0, which needs compatibility verification with .NET 10. Also check that MSBuild tooling supports the new TFM.

**Done when**: .NET 10 SDK verified installed, global.json checked (or confirmed absent), Windows App SDK compatibility confirmed, all required Windows SDKs present.

---

### 02-upgrade-all-projects: Upgrade all projects to .NET 10

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

---

### 03-final-validation: Validate upgraded solution

Build the full solution, run all tests, verify the application launches and core functionality works. Document any behavioral changes or deferred recommendations for post-upgrade work.

Validate MSBuild package generation for both MSIX packages (Production and Beta channels). Ensure pre-commit hooks still pass with the upgraded projects.

**Done when**: Full solution builds successfully (x64 Debug configuration), all tests pass, application launches and basic navigation works, MSIX package artifacts generate successfully, pre-commit hooks pass, any runtime behavioral differences documented.
