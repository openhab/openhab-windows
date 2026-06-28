# .NET Version Upgrade to .NET 10

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0 (.NET 10 LTS)

## Source Control
- **Source Branch**: feature/upgrade-to-winapp11
- **Working Branch**: feature/upgrade-to-winapp11
- **Commit Strategy**: Single Commit at End
- **Branch Sync**: Auto (Merge)

## Upgrade Options
**Source**: .github/upgrades/scenarios/dotnet-version-upgrade/upgrade-options.md

### Strategy
- **Upgrade Strategy**: All-at-Once

### Compatibility
- **Unsupported Packages**: Resolve Inline
- **Unsupported API Handling**: Fix Inline

## Strategy
**Selected**: All-At-Once
**Rationale**: 6 projects all on .NET 9 with moderate complexity; atomic upgrade approach best fits the clear dependency structure and manageable scope.

### Execution Constraints
- Single atomic upgrade — all projects updated together in one pass
- Solution temporarily broken until upgrade completes — all changes are interdependent
- Validate full solution build after upgrade with 0 errors
- Fix upgrade-introduced warnings immediately; pre-existing warnings are deferred (see Key Decisions Log)
- Testing happens after the atomic upgrade completes successfully

## Key Decisions Log
- **Pre-existing warnings deferred** (2025, user chose Option 1) — The ~108 pre-existing nullable (CS86xx) and StyleCop (SAxxxx) warnings pre-date the .NET 10 upgrade (`<Nullable>enable</Nullable>` and StyleCop.Analyzers were already configured). Keeping the upgrade warning-neutral: only the upgrade-introduced NU1510 was fixed. Pre-existing warnings to be handled as a separate focused pass.

## Reminders & Deferred Items
- **Pre-existing nullable + StyleCop warnings** (~108 unique, mostly in openHAB.Windows) — deferred to a separate task. Recommended approach: use the `migrating-csharp-nullable-references` workflow for the CS86xx warnings, and auto-fix StyleCop (SAxxxx) style warnings. Not caused by the .NET 10 upgrade.
