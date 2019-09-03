# Development Environment Setup

## Overview

The samples provided in this repo are a mixture of .NET Core and NodeJS. Depending on your individual developer proclivities :-) you will probably only be interested in a single runtime. Where possible .NET core has been used for the .NET samples. This should execute on Windows, Linux flavours and OSX. The only exception with respect to .NET is a WPF full-fat .NET utility which can be used to obtain Forge Application OAuth access tokens. The prerequisites below can be used ot set up a developer machine from scratch. You will probably want to pick an choose according to the code you are interested in.

## Prerequisites

### Windows Only

- [.NET 4.8](https://dotnet.microsoft.com/download/thank-you/net48-developer-pack) required for building full-fat .NET Forge authentication tooling
- [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) for building full-fat .NET WPF samples and .NET Core sample solutions.

### OS X

- [Visual Studio 2019 for Mac](https://visualstudio.microsoft.com/vs/mac/) for building .NET Core sample solutions.

### Cross Platform

- [Visual Studio Code](https://code.visualstudio.com/) cross platform JavaScript and .NEt Core IDE.
- [.NET Core SDK](https://dotnet.microsoft.com/download) for .NET Core Samples.
- [NodeJS](https://nodejs.org/en/) for JavaScript samples.
- [git](https://git-scm.com/) Git source control.
- [PowerShell Core](https://github.com/PowerShell/PowerShell) Optional.

## MCConfig

MCConfig is a .NET Core utility application for setting up the local developer environment. It should run on any platform supported by .NET core.

### Build and Run MCConfig

Either open the solution `.\tools\dotnet\dotnet-tools.sln` in Visual Studio 2019 and run MCConfig or in a shell of your choice :-

```
$ > cd .\tools\dotnet\src\MCConfig
$ > dotnet restore
$ > dotnet build
$ > dotnet run
```

### Main Menu

On successfully running MCConfig you will be presented with various options. Option 1 allows you to use the values you obtained from [setting up your Forge App](forge-app-setup.md) to configure you local development environment(s). Option 4 allows you to set the default environment (`dev`, `stg` or `prod`). External users will always typically ues `prod`

```
----------------------------------------------------------
    Model Coordination Developer Configuration Utility
----------------------------------------------------------

   1. Set Forge App Configuration
   2. Get Forge App Configuration
   3. Delete all local developer configuration
   4. Set default Forge App configuration

   5. Set cached Forge App auth token
   6. Get cached Forge App auth token
   7. Delete cached Forge App auth token

   8. Configure Azure Cosmo DB Account
   9. Get Azure Cosmo DB Account

  10. Exit

Select :                                                 
```

The MCConfig utility manages the `.nucleus` folder in your user home folder. If you are running PowerShell Core you can find the location of this folder with the following command line :-

```powershell
PS > [System.IO.Path]::Combine([System.Environment]::GetFolderPath('UserProfile'), '.nucleus')
```

Essentially MCConfig maintains the following JSON files

| Path                          | Description                                                    |
| ----------------------------- | -------------------------------------------------------------- |
| `\{user_home}\.nucleus\dev`   | Development environment Fore App Configuration                 |
| `\{user_home}\.nucleus\stg`   | Staging environment Fore App Configuration                     |
| `\{user_home}\.nucleus\prod`  | Production environment Fore App Configuration                  |
| `\{user_home}\.nucleus\token` | Cached OAuth token created by MCAuth WFP authorization utility |

The environment configuration files have the following format :-

```json
{
  "Account": "the-bim-360-account-GUID-you-will-be-using-with-the-samples",
  "Project": "the-bim-360-project-GUID-you-will-be-using-with-the-samples",
  "ForgeBimHubId": "b.the-bim-360-account-GUID-you-will-be-using-with-the-samples",
  "ForgeBimProjectId": "b.the-bim-360-project-GUID-you-will-be-using-with-the-samples",
  "ClientId": "forge-app-client-id",
  "Secret": "forge-app-secret",
  "CallbackUrl": "forge-app-callback-url",
  "IsDefault": true|false
}
```

e.g.

```json
{
  "Account": "f005a243-96e5-41f5-bd4f-bar68a790b0b",
  "Project": "f00cef12-deef-4771-9feb-bar5643e3b0b",
  "ForgeBimHubId": "b.f005a243-96e5-41f5-bd4f-bar68a790b0b",
  "ForgeBimProjectId": "b.f00cef12-deef-4771-9feb-bar5643e3b0b",
  "ClientId": "uioaslkhdlashdgb3udAhOzrnSTp47JZdo",
  "Secret": "HKKFY8AAQrOgAys753",
  "CallbackUrl": "https://foo-bar.com/aok",
  "IsDefault": true
}
```

You are free to create UTF-8 encoded versions of these files manually. Unless you are an Autodesk developer or partner you are unlikely to have access to Autodesk's staging and development environments. These samples have all been created assuming you are using the production (public) version of Autodesk Forge.

---
[back](../README.md)