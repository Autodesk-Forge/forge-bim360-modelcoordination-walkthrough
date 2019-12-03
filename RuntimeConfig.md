# Runtime Configuration Options

Each sample needs the following input configuration **Runtime Configuration**

| Sample | Description |
| --- | --- |
|AuthToken|A valid Forge OAuth 2.0 3-legged authentication token|
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

[demo video of option 1](https://youtu.be/wYhvxt2DLMQ)

### Option 2: SampleConfiguration.json (default)

The default mechanism to configure these sample is via a ` SampleConfiguration.json ` saved to a `.adsk-forge` directory in the user�s default profile directory. The following PowerShell command can be used to determine the location of this directory. For example, on a windows machine this is typically `c:\users\{user_name}` and `/Users/{user_name}` on OSX etc.

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

The [MCConfig](src/MCConfig) tool can be used to do this from the command line. This application will create the `.adsk-forge` folder if it does not exist and write the `SampleConfiguration.json` file.

```powershell
 PS > cd ./src/MCConfig
 PS > dotnet restore
 PS > dotnet build
 PS > dotnet run --accountId={BIM 360 account GUID} --projectId={BIM 360 Project GUID} --authToken={auth token}
```
[demo video of option 2 by MCConfig](https://youtu.be/B2VwfE_d3RQ)

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

[demo video of option 1](https://youtu.be/1lO3mo8BgXI)
