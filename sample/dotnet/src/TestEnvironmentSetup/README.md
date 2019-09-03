# Check Developer Setup

### Demonstrates

This .NET core console application checks that the development environment has been properly configured. It tests that a default hub and project has been selected, that a valid user 3LO OAuth token has been cached and that the Hub (Account) and Project configured are accessible via Forge-DM.

### Dependencies

- [A default ForgeApp has been configured with a Hub (Account) and Project GUID](../../../../doc/forge-app-setup.md)
- A cached OAuth user token has been set, windows users can use [MCAuth](../../../../tools/dotnet/src/MCAuth/README.md)

### Build and Run

```powershell
 PS > cd .\sample\dotnet\src\TestEnvironmentSetup
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run
```

### Code Walk-through

This code makes extensive use of .NET Core's in-built dependency injection framework, [`System.Composition`](https://docs.microsoft.com/en-us/dotnet/api/system.composition). If you are not familiar with this namespace you can find a quick overview [here](../../../../doc/system-composition-explained.md). The [`MCSample.ForgeAppContext`](../MCSample/Forge/ForgeAppContext.cs) class is responsible for composting the dependency injection container.

```csharp
using (var ctx = ForgeAppContext.Create())
{
    ...
}
```

Access to Forge-DM uses the [Autodesk.Forge](https://www.nuget.org/packages/Autodesk.Forge/) public nuget package. The types supplied by this package are wrapped by the [`MCSample.IForgeClient`](../MCSample/Forge/IForgeClient.cs) interface to make them available to `System.Composition` and simplify the developer experience. This code can be used as is to talk to Forge-DM however it lacks a number of features which would be necessary in production environments (retry, circuit breaker, caching, enhanced instrumentation etc..).

```csharp
var client = ctx.ExportService<IForgeClient>();
```

The following tests are performed by this sample.

```csharp
Assert.True(configManager.ConfigDirectory.Exists, $"Could not find sample config folder {configManager.ConfigDirectory.FullName}");

var config = client.Configuration ?? throw new InvalidOperationException("Could not determine default ForgeApp configuration! Have you run MCConfig?");

var token = await client.GetToken() ?? throw new InvalidOperationException("Could not get a cached token! Have you run MCConfig or MCAuth?");

var project = await client.GetProject() ?? throw new InvalidOperationException("Could access a test Account/Project! Have you run MCConfig?");

Assert.True(SampleFileManager.StateDirectory.Exists, $"Could not find tmp state folder {SampleFileManager.StateDirectory.FullName}");

Assert.True(SampleFileManager.SampleDirectory.Exists, $"Could not find sample file folder {SampleFileManager.SampleDirectory.FullName}");

var container = await msClient.GetContainer(forgeClient.Configuration.Project);
```

---
[home](../../../../README.md)