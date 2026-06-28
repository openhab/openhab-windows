# openHAB Windows — Copilot Instructions

## Build

This is a WinUI 3 / .NET 9 app. Use **MSBuild** (not `dotnet build`) to build the app package:

```powershell
# Restore NuGet packages
dotnet restore OpenHAB.Windows.sln /p:BuildWithNetFrameworkHostedCompiler=true

# Build for development (x64 Debug)
msbuild OpenHAB.Windows.sln /p:Platform=x64 /p:Configuration=Debug /t:Rebuild
```

The CI pipeline targets x86, x64, and arm64. To build a store package (like CI):
```powershell
msbuild OpenHAB.Windows.sln /p:Platform=x86 /p:AppxBundlePlatforms="x86|x64|arm64" /p:AppxBundle=Always /p:UapAppxPackageBuildMode=StoreUpload /p:GenerateAppxPackageOnBuild=true /p:Configuration=Release /t:Rebuild
```

Required SDKs: **Windows 10 SDK 10.0.20348.0** and **Windows 11 SDK 10.0.26100.0**.

## Architecture

Four C# projects + two MSIX packaging projects:

| Project | Role |
|---|---|
| `openHAB.Common` | Shared resources (`AppResources`) |
| `openHAB.Core.Client` | openHAB REST API client, models, SSE events, connection management |
| `openHAB.Core` | Application services (SitemapService, AppManager, ItemManager, IconCaching, Notifications) |
| `openHAB.Windows` | WinUI 3 app — Views, ViewModels, widget controls, DI setup |
| `openHAB.Windows.Package` / `.Beta` | MSIX packaging for Store (Production / Beta channels) |

**Data flow**: `openHAB.Core.Client` → REST/SSE → openHAB server. `openHAB.Core` services orchestrate the client. `openHAB.Windows` ViewModels consume services via DI. Views bind to ViewModels.

**Real-time updates** arrive via Server-Sent Events (SSE) on the `/events` endpoint, parsed by `IOpenHABEventParser`, then dispatched as `UpdateItemMessage` / `ItemStateChangedMessage` via `StrongReferenceMessenger`.

**Connection modes**: Local, Remote, and Demo. `IConnectionService` auto-detects the active connection, then `OpenHABClient` holds a typed `HttpClient` (`"local"` or `"remote"`) for the session.

## Key Conventions

### MVVM & ViewModels
- All ViewModels extend `ViewModelBase<TModel>`, which wraps a model and implements `INotifyPropertyChanged`.
- Use the `Set(ref _field, value)` helper (not manual `OnPropertyChanged`) for bindable properties.
- Commands use the custom `ActionCommand` class (not `RelayCommand`). Pattern:
  ```csharp
  public ActionCommand MyCommand => _myCommand ?? (_myCommand = new ActionCommand(ExecuteMyCommand, CanExecuteMyCommand));
  ```
- ViewModels with async initialization use a **static async factory** method (`CreateAsync`) instead of constructor logic. Keep constructors synchronous.

### Messaging (inter-component communication)
- Use `StrongReferenceMessenger.Default` from `CommunityToolkit.Mvvm.Messaging` for all cross-component messages.
- Register in the constructor; **always unregister in `Dispose()`** (see `SitemapViewModel` for the pattern).
- Channel-scoped messages use a string token (e.g., `sitemap.Name`) as the second type argument.

### Widget Controls
- Each widget type is a `UserControl` in `src/openHAB.Windows/Controls/`, extending the abstract `WidgetBase`.
- `WidgetBase` exposes a `Widget` DependencyProperty of type `WidgetViewModel`.
- New widget controls **must** implement the abstract `SetState()` method, which is called on the UI thread when item state changes.
- Dispatch UI updates via `App.DispatcherQueue.EnqueueAsync(...)`.

### Dependency Injection
- DI is configured in `AppServiceExtensions.cs`.
- Services are registered as **Singletons**; ViewModels as **Transient**; Views as **Singleton**.
- Resolve services inside widgets/controls via the `IServiceProvider` passed through `WidgetViewModel`.

### Logging
- Use `ILogger<T>` injected via constructor. NLog writes JSON logs to `AppPaths.LogsDirectory`.
- Use structured logging: `_logger.LogInformation("Loading '{ItemName}'", itemName)` — not string interpolation.

### Error handling
- Wrap REST API failures in `OpenHABException` and send a `ConnectionErrorMessage` via the messenger.
- `HttpResponseResult<T>` is the return type for operations that need both a result and status code.

## Coding Standards

- Follow [.NET Core Foundational libraries coding guidelines](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md).
- **All public members require XML documentation comments.**
- Fix any Roslyn analyzer warnings before submitting a PR.

## Pre-commit Hooks

Install the pre-commit tooling once:
```powershell
.\setup_pre-commit.ps1
```

Hooks enforce: trailing whitespace, EOF newline, YAML validity, large file check, and **DCO sign-off** on every commit:
```
Signed-off-by: Your Name <your.email@example.com> (github: your_handle)
```

## Branch & Commit Conventions

- Branch names: `<issue-number>-short-description` (e.g., `42-fix-slider-widget`).
- Commit subject: capitalized, imperative, ≤50 chars (e.g., `Fix slider widget step rounding`).
- Reference issues in commits: `Fixes #42` or `Closes #42`.
- Squash commits into logical units before merge (`git rebase -i`).
