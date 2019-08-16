# Working With Model Set Versions

### Demonstrates

This .NET core console application demonstrates how to query a model set for model set versions. It builds on the [CreateModelSet](../CreateModelSet/README.md) sample, which creates a test model set and uploads three sample files. In this sample app were are going to interrogate the model set created by the [CreateModelSet](../CreateModelSet/README.md) sample app.

### Dependencies

- A cached OAuth user token has been set
- [TestEnvironmentSetup](../TestEnvironmentSetup/README.md) executed successfully
- [CreateModelSet](../CreateModelSet/README.md) executed successfully

### Build and Run

```powershell
 PS > cd .\sample\dotnet\src\QueryModelSet
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run
```

### Code Walk-through

The first step is to load the cached state created by the [CreateModelSet](../CreateModelSet/README.md) sample app.

```csharp
state = await SampleFileManager.LoadSavedState<CreateModelSetState>();
```

To retrieve the model set created by the [CreateModelSet](../CreateModelSet/README.md) sample app we use the `IModelSetClient` which in turn wraps the `IScopesClientV3` from the [`Autodesk.Nucleus.Scopes.Client.V3`](https://www.nuget.org/packages/Autodesk.Nucleus.Scopes.Client.V3) nuget package.

```csharp
var mcClient = ctx.ExportService<IModelSetClient>();

var ms = await mcClient.GetModelSet(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);
```

Once we have a model set we can grab the versions...

```csharp
versions = await mcClient.GetModelSetVersions(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);
```

Versions can be retrieved by version number... 

```csharp
var version = await mcClient.GetModelSetVersion(state.ModelSet.ContainerId, state.ModelSet.ModelSetId, 1U);
```

...or by requesting the latest version

```csharp
var latest = await mcClient.GetLatestModelSetVersion(state.ModelSet.ContainerId, state.ModelSet.ModelSetId);
```

---
[home](../../../../README.md)