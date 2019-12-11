# BIM 360 Model Coordination API Walkthrough in .NET Core 


[![.NET Core 3.1](https://img.shields.io/badge/.NET%20Core-3.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet-core/3.0)
[![visual code 2019](https://img.shields.io/badge/visual%20studio%202019-16.4.0-orange.svg)](https://visualstudio.microsoft.com/)
[![visual code](https://img.shields.io/badge/visual%20code-1.28.2-orange.svg)](https://code.visualstudio.com)

[![oAuth2](https://img.shields.io/badge/oAuth2-v1-green.svg)](https://forge.autodesk.com/en/docs/oauth/v2/overview/)
[![Data-Management](https://img.shields.io/badge/Data%20Management-v1-green.svg)](https://forge.autodesk.com/en/docs/data/v2/developers_guide/overview/)
[![Viewer](https://img.shields.io/badge/Viewer-v7-green.svg)](https://forge.autodesk.com/en/docs/viewer/v7/developers_guide/overview/)
[![BIM-360](https://img.shields.io/badge/BIM%20360-v1-green.svg)](https://forge.autodesk.com/en/docs/bim360/v1/overview/introduction/) 

[![BIM 360 Model Set](https://img.shields.io/badge/ModelSetAPI-3.0.65-orange)]( https://www.nuget.org/packages/Autodesk.Forge.Bim360.ModelCoordination.Modelset/)
[![BIM 360 Clash](https://img.shields.io/badge/ClashAPI-3.3.27-yellowgreen)]( https://www.nuget.org/packages/Autodesk.Forge.Bim360.ModelCoordination.Clash/)
[![BIM 360 Index](https://img.shields.io/badge/IndexAPI-1.2.44-green)]( https://www.nuget.org/packages/Autodesk.Forge.Bim360.ModelCoordination.Index/)

[![License](http://img.shields.io/:license-mit-red.svg)](http://opensource.org/licenses/MIT)
[![Level](https://img.shields.io/badge/Level-Intermediate-blue.svg)](http://developer.autodesk.com/)

## Description

This repository demonstrates basic scenarios of Model Coordination API in .NET Core, including model sets, clash, property index and clash issue, etc. It also provides a demo on how to classify the raw data, applying them in further comprehensive scenarios.
 
## Available Samples

| Order | Sample | Description | Demo |
| --- | --- | --- | --- |
|1|[Environment Check](/samples/1.%20TestEnvironmentSetup/Program.cs)|A sanity check of the current developer environment|[help](/help/1.%20TestEnvironmentSetup.md)
|2|[Create Model Set](/samples/2.%20CreateModelSetSample/Program.cs)|Creating a model set from first principals|[help](/help/2.%20CreateModelSetSample.md)
|3|[Model Set Versions](/samples/3.%20GetModelSetAndVersionsSample/Program.cs)|Querying model set versions via version number and tip|[help](/help/3.%20GetModelSetAndVersionsSample.md)
|4|[Clash Results](/samples/4.%20GetClashResultsSample/Program.cs)|Working with clash test result resources|[help](/help/4.%20GetClashResultsSample.md)
|5|[Assigned and Closed Clash](/samples/5.%20QueryModelSetVersionIndexSample/Program.cs)|Querying assigned/closed clash groups and issue details|[help](/help/5.%20QueryModelSetVersionIndexSample.md)
|6|[Model Set Indexes](/samples/6.%20CreateAndQueryViewsSample/Program.cs)|BIM property querying against a model set version index|[help](/help/6.%20CreateAndQueryViewsSample.md)
|7|[Model Set Views](/samples/7.%20ClassifyClashingObjectsSample/Program.cs)|Create and query mode set custom views|[help](/help/7.%20ClassifyClashingObjectsSample.md)
|8|[Classify Clashing Objects](/samples/8.%20AssignedAndClosedClashGroupSample/Program.cs)|Use BIM property index to classify clashing objects|[help](/help/8.%20AssignedAndClosedClashGroupSample.md)

The samples in this repository build on one another. The execution order above matters. After [Create Model Set](/samples/2.%20CreateModelSetSample/Program.cs), please wait some time for coordinating process completes. Then run the other samples.
 
If you have not used the sample which creates a model set then you will not be able to use the remaining samples without first tweaking the model set input variables. 

## Setup

### Prerequisites

1. [Knowledge of model coordination](https://knowledge.autodesk.com/support/bim-360/learn-explore/caas/CloudHelp/cloudhelp/ENU/BIM360D-Model-Coordination/files/GUID-38CC3A1C-92FF-4682-847F-9CFAFCC4CCCE-html.html) 
2. **.NET Core**: The console applications currently target `netcoreapp3.1` and have a single `netstandard2.1` dependency. You will need [.NET Core 3.1 SDK]( https://dotnet.microsoft.com/download/dotnet-core/3.1) installed. The samples can also work with `netcoreapp3.0`, but since [AuthWeb ASP.NET Core Blazor WebApp](/samples/auxiliary/AuthWeb) depends on `netcoreapp3.1`, it is recommended use `netcoreapp3.1` with all samples. To test on cross-platform, ensure [.NET Core 3.1 SDK]( https://dotnet.microsoft.com/download/dotnet-core/3.1) has been installed.  

3. **Forge Account**: Learn how to create a Forge Account, activate subscription and create an app by [this tutorial](http://learnforge.autodesk.io/#/account/). Get _Forge client id_, _Forge client secret_ and _Forge callback url_. These can be used by **option #3** (in [RuntimeConfig.md](RuntimeConfig.md)) to get 3-legged token. Please register Forge app with the _callback url_ as 

    ```http://localhost:3000/api/forge/callback/oauth```

4. **Postman**: This is optional, but recommendable to get 3-legged token. Check the tutorial of [Postman Scripts of Model Coordination API](https://github.com/xiaodongliang/bim360-mcapi-postman.test)for details.

5. **BIM 360 Account and project**: must be Account Admin to add the app integration. [Learn about provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps). Make a not with the _account id_ and  _project id_.

6. Ensure [Model Coordination](https://knowledge.autodesk.com/support/bim-360/learn-explore/caas/CloudHelp/cloudhelp/ENU/BIM360D-Model-Coordination/files/GUID-38CC3A1C-92FF-4682-847F-9CFAFCC4CCCE-html.html) module has been activated in BIM 360 project.
  

### Running Locally

1. Clone this project or download it. It's recommended to install [GitHub desktop](https://desktop.github.com/). To clone it via command line, use the following (**Terminal** on MacOSX/Linux, **Git Shell** on Windows):

    ```git clone https://github.com/xiaodongliang/bim360-model-coordination-netCore.tutorial```

    This repo contains large RVT sample files and uses [`git lfs`](https://git-lfs.github.com/), make sure you clone accordingly

2. Open the solution file in **Visual Studio**, ensure dependent packages of nuGet have been installed, and the dependent project [Forge ](/samples/auxiliary/Forge) has been built and imported to the sample projects successfully. 

3. Each sample needs the following input configuration **Runtime Configuration**

      | Sample | Description |
      | --- | --- |
      |AuthToken|A valid Forge OAuth 2.0 3-legged authentication token|
      |AccountId|The BIM 360 Hub (Account) GUID to be used by the samples|
      |ProjectId|The BIM 360 Project GUID to be used by the samples|some variables will 

Three options are available with this walkthough sample. Please check [RuntimeConfig.md](RuntimeConfig.md) for detail steps.

4. Follow the steps in [readme of each sample](/help) to run the samples.

5. The samples can be also run on cross platform. 

      5.1 ) Open the sample folder in **Visual Studio Code**.  The samples in this repo consist of a suite of .NET Core Console applications and a web application. To build and run these applications you will require the cross-platform .NET Core SDK. 

      5.2) In terminal, switch to the folder of the specific sample, type `dotnet restore` to install the required packages and type `dotnet build` to build the sample:

      ```powershell
      PS > cd ./samples/{sample_folder}
      PS > dotnet restore
      PS > dotnet build
      ```
 
      5.3) After the steps above, type `dotnet run` in the terminal to run the sample.
      ```powershell
        PS > cd ./samples/{sample_folder}
        PS > dotnet run
      ```

## Further Reading
- [Model Coordination API]( https://forge.autodesk.com/en/docs/bim360/v1/reference/http/mc-modelset-service-v3-create-model-set-POST/)
- [Model Coordination API SDK]( https://www.nuget.org/packages/Autodesk.Forge.Bim360.ModelCoordination.ModelSet/) 
- [BIM 360 API](https://forge.autodesk.com/en/docs/bim360/v1/overview/) and [App Provisioning](https://forge.autodesk.com/blog/bim-360-docs-provisioning-forge-apps)
- [Data Management API](https://forge.autodesk.com/en/docs/data/v2/overview/)

## Tutorials
- [Model Coordination API Document](https://forge.autodesk.com/en/docs/bim360/v1/tutorials/model-coordination)
- [Model Coordination API Node.js Tutorials]( https://github.com/xiaodongliang/bim360-mcapi-node-unit.test)

## License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

## Written by
-	[Don Whittle](https://www.linkedin.com/in/don-whittle-4869088), Model Coordination engineering team, Autodesk.
-	reviewed by Xiaodong Liang [@coldwood](https://twitter.com/coldwood), [Forge Partner Development](http://forge.autodesk.com),Autodesk
