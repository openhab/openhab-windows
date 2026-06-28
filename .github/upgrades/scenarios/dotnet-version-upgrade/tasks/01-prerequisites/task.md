# 01-prerequisites: Verify SDK and tooling compatibility

Validate that the .NET 10 SDK is installed and accessible, and check for any global.json constraints that could block the upgrade. This includes verifying Windows App SDK compatibility with .NET 10 and ensuring required Windows SDKs (10.0.20348.0 and 10.0.26100.0) are available.

The solution uses Windows App SDK 2.2.0, which needs compatibility verification with .NET 10. Also check that MSBuild tooling supports the new TFM.

**Done when**: .NET 10 SDK verified installed, global.json checked (or confirmed absent), Windows App SDK compatibility confirmed, all required Windows SDKs present.
