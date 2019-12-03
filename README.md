# BIM 360 Model Coordination API .NET Core Tutorial


![.NET Core 3.0](https://img.shields.io/badge/.NET%20Core-3.0-blue.svg) 
![.NET Core 3.1 Preview 2](https://img.shields.io/badge/.NET%20Core-3.1-Preview-2-blue.svg) 

[![visual code](https://img.shields.io/badge/visual%20code-1.28.2-orange.svg)](https://code.visualstudio.com)

[![oAuth2](https://img.shields.io/badge/oAuth2-v1-green.svg)](https://forge.autodesk.com/en/docs/oauth/v2/overview/)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v1-green.svg)](https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/)
[![Viewer](https://img.shields.io/badge/Viewer-v7-green.svg)](https://forge.autodesk.com/en/docs/viewer/v7/developers_guide/overview/)
[![BIM-360](https://img.shields.io/badge/BIM%20360-v1-green.svg)](https://forge.autodesk.com/en/docs/bim360/v1/overview/introduction/) 

[![ModelSetAPI](https://img.shields.io/badge/ModelSetAPI-3.0.65-orange)](https://www.npmjs.com/package/forge-bim360-modelcoordination-modelset)
[![ClashAPI](https://img.shields.io/badge/ClashAPI-3.3.27-yellowgreen)](https://www.npmjs.com/package/forge-bim360-modelcoordination-clash)
[![IndexAPI](https://img.shields.io/badge/IndexAPI-1.2.44-green)](https://www.npmjs.com/package/forge-bim360-modelcoordination-index)

[![License](http://img.shields.io/:license-mit-red.svg)](http://opensource.org/licenses/MIT)
[![Level](https://img.shields.io/badge/Level-Intermediate-blue.svg)](http://developer.autodesk.com/)

## Description

This repository demonstrates basic scenarios of Model Coordnation API in .NET Core, including modelsets, clash, property index and clash issue etc.
 

## Thumbnail

  <p align="center"><img src="./help/main.png" width="1000"></p>  
 

## Available Samples

| Order | Sample | Demonstrates |
| --- | --- | --- |
|1|[Environment Check](src/TestEnvironmentSetup/Program.cs)|A sanity check of the current developer environment|
|2|[Create Model Set]( src/CreateModelSetSample/Program.cs)|Creating a model set from first principals|
|3|[Model Set Versions](src/GetModelSetAndVersionsSample/Program.cs)|Querying model set versions via version number and tip|
|4|[Clash Results](src/GetClashResultsSample/Program.cs)|Working with clash test result resources|
|5|[Assigned and Closed Clash](src/AssignedAndClosedClashGroupSample/Program.cs)|Querying assigned/closed clash groups and issue details|
|6|[Model Set Indexes](src/QueryModelSetVersionIndexSample/Program.cs)|BIM property querying against a model set version index|
|7|[Model Set Views](src/CreateAndQueryViewsSample/Program.cs)|Create and query mode set custom views|
|8|[Classify Clashing Objects](src/ClassifyClashingObjectsSample/Program.cs)|Use BIM property index to classify clashing objects|

The samples in this repo build on one another. The execution order above matters. If you have not used the sample which creates a model set then you will not be able to use the remaining samples without first tweaking the model set input variables. To play with previous modelsets, please note:

```diff
-     Note: The logic of this sample works for ModelSet which are created after Oct 1st,2019
```

## Setup

### Prerequisites

1. **.NET Core**: The console applications currently target `netcoreapp3.0` and have a single `netstandard2.1` dependency. If you would like to try the [AuthWeb ASP.NET Core Blazor web application](src/AuthWeb) for configuring the samples and obtaining a Forge token the you will need to install the latest .NET Core 3.1 SDK (currently preview 2.0). .NET Core 3.1 is likely to be the next LTS release from Microsoft (see the following [blog post]( https://devblogs.microsoft.com/dotnet/announcing-net-core-3-1-preview-2/))

- This repo contains large RVT sample files and uses [`git lfs`](https://git-lfs.github.com/), make sure you clone accordingly :warning:
- [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0) for .NET Core Samples.

Optional :-

- [.NET Core 3.1 Preview 2 SDK]( https://dotnet.microsoft.com/download/dotnet-core/3.1) for AuthWeb support (ASP.NET Blazor)
- [PowerShell Core]( https://github.com/PowerShell/PowerShell) useful .NET command shell
- [Postman](https://www.getpostman.com/)

2. **BIM 360 Account**: must be Account Admin to add the app integration. [Learn about provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps). Make a not with the _account id_ and  _project id_. 

3. **Forge Account**: Learn how to create a Forge Account, activate subscription and create an app at [this tutorial](http://learnforge.autodesk.io/#/account/). Get _Forge client id_, _Forge client secret_ and _Forge callback url_. 

4. In this sample, to test with.

5. Optional: If test with existing modelset, create some [modelsets of Model Coordination](https://knowledge.autodesk.com/support/bim-360/learn-explore/caas/CloudHelp/cloudhelp/ENU/BIM360D-Model-Coordination/files/GUID-38CC3A1C-92FF-4682-847F-9CFAFCC4CCCE-html.html) in BIM 360. Make a note with the _modelset id_. 

  <p align="center"><img src="./help/msid.png" width="1000"></p>  

### Running locally

1. Clone this project or download it. It's recommended to install [GitHub desktop](https://desktop.github.com/). To clone it via command line, use the following (**Terminal** on MacOSX/Linux, **Git Shell** on Windows):

    git clone https://github.com/xiaodongliang/bim360-mcapi-node-pdf.exporter.sample

2. Open the project folder in **Visual Studio Code**.  The samples in this repo consist of a suite of .NET Core Console applications and a web application. To build and run these applications you will require the cross-platform .NET Core SDK. These samples also ship with a Microsoft Visual Studio solution; however **Visual Studio** is not a prerequisite for running this sample code.

3. In terminal, swtich to the folder of the specific sample, type **dotent restore** to install the required packages and type **dotent build** to build the 
```powershell
 PS > cd ./src/{sample_folder}
 PS > dotnet restore
 PS > dotnet build
```
4. Each sample needs the following input configuration **Runtime Configuration**

| Sample | Description |
| --- | --- |
|AuthToken|A valid Forge OAuth 2.0 3-legged authentication token|
|AccountId|The BIM 360 Hub (Account) GUID to be used by the samples|
|ProjectId|The BIM 360 Project GUID to be used by the samples|some variables will 

Three options are available with this tutorial. Please check [RuntimeConfig.md](RuntimeConfig.md) for detail steps.

5. After the steps above, type **dotnet run** in the terminal


4. Set the environment variables with your client ID & secret and finally start it. Via command line, navigate to the folder where this repository was cloned and use the following:   
## `dotnet run`

To run the samples :-

```powershell
 PS > cd ./src/{sample_folder}
 PS > dotnet run
```


## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by
[Don Whittle](https://www.linkedin.com/in/don-whittle-4869088), Model Coordination engineering team, Autodesk.
reviewed by Xiaodong Liang [@coldwood](https://twitter.com/coldwood), [Forge Partner Development](http://forge.autodesk.com),Autodesk 