Set-Location $PSScriptRoot;

try
{
	& dotnet clean;
	& dotnet restore;
	& dotnet build;
	& dotnet publish;

	& dotnet run "$PSScriptRoot\src\AuthWeb\bin\Debug\netcoreapp3.1\publish\AuthWeb.dll";
}
finally
{
	Pop-Location;
}