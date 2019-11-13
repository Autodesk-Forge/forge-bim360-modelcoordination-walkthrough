# BIM 360 Model Coordination API .NET Core Samples

## Disclaimer

```csharp
/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
```

## Available Samples

| Sample | Demonstrates |
| --- | --- |
|[Environment Check](src/TestEnvironmentSetup/Program.cs)|A sanity check of the current developer environment|
|[Create Model Set]( src/CreateModelSetSample/Program.cs)|Creating a model set from first principals|
|[Model Set Versions](src/GetModelSetAndVersionsSample/Program.cs)|Querying model set versions via version number and tip|
|[Clash Results](src/GetClashResultsSample/Program.cs)|Working with clash test result resources|
|[Assigned and Closed Clash](src/AssignedAndClosedClashGroupSample/Program.cs)|Querying assigned/closed clash groups and issue details|
|[Model Set Indexes](src/QueryModelSetVersionIndexSample/Program.cs)|BIM breakdown querying against a model set version|
|[Model Set Views](src/CreateAndQueryViewsSample/Program.cs)|Create and query mode set custom views|

## Building

The samples in this repo consist of a suite of .NET Core Console applications and a web application. To build and run these applications you will require the cross-platform .NET Core SDK. These samples also ship with a Microsoft Visual Studio solution; however Visual Studio is not a prerequisite for running this sample code.

### Prerequisites

The console applications currently target `netcoreapp3.0` and have a single `netstandard2.1` dependency. If you would like to try the AuthWeb ASP.NET Core Blazor web application for configuring the samples and obtaining a Forge token the you will need to install the latest .NET Core 3.1 SDK (currently preview 2.0). .NET Core 3.1 is likely to be the next LTS release from Microsoft (see the following [blog post]( https://devblogs.microsoft.com/dotnet/announcing-net-core-3-1-preview-2/))

- This repo contains large RVT sample files and uses [`git lfs`](https://git-lfs.github.com/), make sure you clone accordingly :warning:
- [.NET Core 3.0 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.0) for .NET Core Samples.

Optional :-

- [.NET Core 3.1 Preview 2 SDK]( https://dotnet.microsoft.com/download/dotnet-core/3.1) for AuthWeb support (ASP.NET Blazor)
- [PowerShell Core]( https://github.com/PowerShell/PowerShell) useful .NET command shell
- [Postman](https://www.getpostman.com/)

### `dotent build`

To compile individual samples :-

```powershell
 PS > cd ./src/{sample_folder}
 PS > dotnet restore
 PS > dotnet build
```

## Runtime Configuration Options

Each sample needs the following input configuration :-

| Sample | Description |
| --- | --- |
|AuthToken|A valid Forge OAuth 2.0 3LO authentication token|
|AccountId|The BIM 360 Hub (Account) GUID to be used by the samples|
|ProjectId|The BIM 360 Project GUID to be used by the samples|

There are three options for managing these values.

### Option 1: In-line

The simplest method is to pass these variables individually to each sample application. At the top of each `Program.cs` file you will see a call to `configuration.Load` commented out. If you uncomment this call, replacing the existing call to `configuration.Load()` you can manually pass these valuses for each sample.

```csharp
    configuration.Load(new Dictionary<string, string>
        {
            { "AuthToken", "Your Forge App OAuth token" },
            { "AccountId", "Your BIM 360 account GUID (no b. prefix)" },
            { "ProjectId", "Your BIM 360 project GUID (no b. prefix)"}
        });
```

### Option 2: SampleConfiguration.json (default)

The default mechanism to configure these sample is via a ` SampleConfiguration.json ` saved to a `.adsk-forge` directory in the user’s default profile directory. The following PowerShell command can be used to determine the location of this directory. For example, on a windows machine this is typically `c:\users\{user_name}` and `/Users/{user_name}` on OSX etc.

```powershell
 PS > [System.IO.Path]::Combine([System.Environment]::GetFolderPath('UserProfile'), '.adsk-forge')
```

To use the default `configuration.Load()` in the samples create a `.adsk-forge` folder and add a `SampleConfiguration.json` UTF-8 configuration file.

```json
{
  "AccountId": "Your BIM 360 account GUID (no b. prefix)",
  "ProjectId": "Your BIM 360 project GUID (no b. prefix)",
  "AuthToken": "Your Forge App OAuth token"
}
```

The MCConfig application can be used to do this from the command line. This application will create the `.adsk-forge` folder if it does not exist and write the `SampleConfiguration.json` file.

```powershell
 PS > cd ./src/MCConfig
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run --accountId={BIM 360 account GUID} --projectId={BIM 360 Project GUID} --authToken={auth token}
```

### Option 3: Using AuthWeb (Experimental .NET Core 3.1)

The AuthWeb ASP.NET Core Blazor WebApp provides a GUI for writing the `SampleConfiguration.json` file and can also be used to request a Forge 3LO OAuth 2.0 authentication token. To use this application you will need to supply your Forge ClientId and Secret. AuthWeb writes these values to `SampleConfiguration.json` and can be used to set the value of AuthToken in this file. To run this application you need to set the callback URL of your Forge application to `https://localhost:5001/signin/oauth/callback`. Once you have updated your application configuration in Forge start the ASP.NET Core web server and browse to `https://localhost:5001`.

```powershell
 PS > cd ./src/AuthWeb
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run
``` 

## `dotnet run`

To run the samples :-

```powershell
 PS > cd ./src/{sample_folder}
 PS > dotnet run
```