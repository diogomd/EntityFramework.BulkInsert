set VERSION=6.1.0.4

pushd .
cd build

call build -f '4.6' -parameters @{ 'version' = '%VERSION%' }

popd

.\tools\nuget\nuget push dist\NuGet\EF6.BulkInsert\EF6.BulkInsert.%VERSION%.nupkg -Source https://api.nuget.org/v3/index.json
.\tools\nuget\nuget push dist\NuGet\EF6.BulkInsert.Hana\EF6.BulkInsert.Hana.%VERSION%.nupkg -Source https://api.nuget.org/v3/index.json
.\tools\nuget\nuget push dist\NuGet\EF6.BulkInsert.MySql\EF6.BulkInsert.MySql.%VERSION%.nupkg -Source https://api.nuget.org/v3/index.json
.\tools\nuget\nuget push dist\NuGet\EF6.BulkInsert.SqlServer\EF6.BulkInsert.SqlServer.%VERSION%.nupkg -Source https://api.nuget.org/v3/index.json
.\tools\nuget\nuget push dist\NuGet\EF6.BulkInsert.Oracle\EF6.BulkInsert.Oracle.%VERSION%.nupkg -Source https://api.nuget.org/v3/index.json