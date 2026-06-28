# Upgrade Options — openHAB Windows

Assessment: 6 projects (net9.0), 2 incompatible packages, 261 API issues, moderate complexity

## Strategy

### Upgrade Strategy
6 projects with moderate complexity (2 incompatible packages, 261 API issues); scope is manageable for atomic upgrade.

| Value | Description |
|-------|-------------|
| **All-at-Once** (selected) | Upgrade all projects simultaneously in a single atomic pass — fastest approach with no multi-targeting overhead |
| Top-Down | Upgrade applications first, multi-target libraries temporarily to keep solution buildable throughout |

## Compatibility

### Unsupported Packages
2 incompatible packages detected (Mapsui.WinUI, Microsoft.Xaml.Behaviors.WinUI.Managed) — small count allows inline resolution.

| Value | Description |
|-------|-------------|
| **Resolve Inline** (selected) | Research and resolve each incompatible package within the same task — no deferred work |
| Defer Resolution | Generate minimal type stubs to make projects compile, create follow-up tasks for real replacements |
| Compatibility Mode | Keep .NET Framework reference with compat shims — may cause runtime failures |

### Unsupported API Handling
261 API issues detected, mostly WinUI type compatibility (Windows.UI.Color, Windows.Foundation.Point) with known migration patterns.

| Value | Description |
|-------|-------------|
| **Fix Inline** (selected) | Resolve all API changes in the same task, including complex ones — no deferred work |
| Defer Complex Changes | Apply simple replacements inline, generate stubs for complex changes and create resolution subtasks |
