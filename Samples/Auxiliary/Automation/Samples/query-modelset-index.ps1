# make sure you run build-and-import-module.ps1 before attempting to run this sample. The following
# cmdlets also assume that you have persisted a token in the Sampleconfiguraiton.json file. You can 
# use the AuthWeb ASP.NET Blazor web application to sign into Forge and obtain a token.

$container = 'f83cef12-deef-4771-9feb-4f85643e3c46';
$modelset = 'b412a75f-bcfa-4566-a8ef-edb974709817';
$verison = 2;

# search the index fields for the name field
$searchText = '__name__';

$fields = Search-ModelSetIndexFields -Container $container `
                                     -ModelSet $modelset `
                                     -Version $verison `
                                     -SearchText $searchText;

# display our search results as CSV - there should only be 1 ;-)
#$fields | ConvertTo-Csv;

$allFields = Get-ModelSetIndexFields -Container $container `
                                     -ModelSet $modelset `
                                     -Version $verison;

# run a query
$query = "select * from s3object s where s.p6637df3c = 'Concrete - Cast-in-Place Concrete - Footing'";
#$query = "select sum(s.p2707f0a3 * s.p2b0fd73f * s.p15f8f5ec) + sum(3.14 * ((s.p8fcbb8f7 / 2)*(s.p8fcbb8f7 / 2)) * s.pca1638c2) as volume from s3object s"

"Run $query";

$resultPath = New-ModelSetIndexQuery -Container $container `
                                     -ModelSet $modelset `
                                     -Version $verison `
                                     -Query $query;

# stream the results into memory as IndexRow objects
# and print the name field
$rows = Get-IndexRows -Path $resultPath;

$objects = $rows | %{ $_.Data.ToString() } | ConvertFrom-Json;

$objects;