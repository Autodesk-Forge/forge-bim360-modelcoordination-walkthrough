# set the paths we will use to build and load the module
# ise Path.Combine to make sure this script works cross platform
$project = [System.IO.Path]::Combine($PSScriptRoot, '..', 'Forge.Automation.csproj');
$module = [System.IO.Path]::Combine($PSScriptRoot, '..', 'bin', 'Debug', 'netstandard2.0', 'publish', 'Forge.Automation.dll');

& dotnet build $project;
& dotnet publish $project;

# load the module
Import-Module $module;

# list the commands in the module
Get-Module | Where-Object -Property Name -Value Forge.Automation -EQ | %{ Get-Command -Module $_ };