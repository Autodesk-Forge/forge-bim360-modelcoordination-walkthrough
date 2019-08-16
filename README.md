# Model Coordination API Documentation

## Concepts

Before getting started with the Model Coordination API users are encouraged to familiarise themselves with some of the key concepts.  

- [Model Sets and Model Set Versions](doc/model-sets.md)
- [Clash Testing and Grouping](doc/clash.md)
- [`System.Composition`](doc/system-composition-explained.md) explained (.NET core only!)

## Getting Started, Machine Setup and Developer Tools

- [Creating and registering a Forge Application with a BIM 360 Account](doc/forge-app-setup.md)
- [Setting up the developer environment](tools/dotnet/src/MCConfig/README.md)
- [Using MCAuth on Windows to manage cached tokens](tools/dotnet/src/MCAuth/README.md)

## Samples

Before you attempt to run any of the samples make sure you have followed the [Setting up the developer environment](doc/dev-machine-setup.md) instructions.

### .NET Core

All of the .NET Core samples are console applications. If you are using Visual Studio then you can view and run the samples by opening :-

```PowerShell
PS > ii .\sample\dotnet\mc-sample.sln
```

The samples are designed to build on one another's output and therefore should be executed in odder. If you are not using Visual Studio you will find `dotnet` instructions for running each sample from the command line in the following pages :-


| Sample | Description |
| --- | --- |
| [Environment setup check](sample/dotnet/src/TestEnvironmentSetup/README.md)|Not strictly a sample. Instead it does a sanity check of the current developer environment to determine if samples can execute|
|[Creating and Querying Model Sets](sample/dotnet/src/CreateModelSet/README.md)|How to create a model set and upload files to the model set service|
|[Creating and Querying Model Set Views](sample/dotnet/src/ModelSetViews/README.md)|How to create and modify model set views|
|[Querying Model Set Versions](sample/dotnet/src/QueryModelSet/README.md)|How to query model set versions for a give model set, including querying a specific model set version or the latest tip version.|
|[Querying Model Set Version Clash Results](sample/dotnet/src/QueryClashTestResults/README.md)|How to work with the different resource files comprising the clash test results for a give model set version|
|[Querying Model Set Indexes](sample/dotnet/src/QueryModelSetVersionIndex/README.md)|How to run BIM property queries against model set BIN property indexes|
|[Putting it all together: Azure Cosmos DB & Power BI](sample/dotnet/src/CosmoDbUploader/README.md)|How to use the APIs to classify clash results, upload these data to Azure Cosmos and visualize clash in Microsoft Power BI|
|[Putting it all together: Assigned & Closed Clash Groups](sample/dotnet/src/QueryAssignedClosedClash/README.md)|How to use the APIs to query clashs which have been assigned to BIM 360 issues or marked as closed|

### Postman

The [Postman](https://www.getpostman.com/) sample collection [included in this repository](sample/postman/mcsample.postman_collection.json) in limited to `HTTP GET` operations and therefore only partially demonstrates the capabilities of the BIM 360 Model Coordination APIs.

### NodeJS

