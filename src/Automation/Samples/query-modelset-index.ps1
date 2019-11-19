# set the environment up
. ([System.IO.Path]::Combine($PSScriptRoot, 'build-and-import-module.ps1'));

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
$fields | ConvertTo-Csv;

# run a query
$query = 'select * from s3object s where count(s.docs) > 0 limit 100';

$resultPath = New-ModelSetIndexQuery -Container $container `
                                     -ModelSet $modelset `
                                     -Version $verison `
                                     -Query $query;

$resultpath.FullName;

# stream the results into memory as IndexRow objects
# and print the name field
$rows = Get-IndexRows -Path $resultPath;

$rows | %{ [string]($_.Data[$fields[0].Key]) };