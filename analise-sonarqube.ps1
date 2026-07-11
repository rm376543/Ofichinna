$token = "sqp_cf8689b77d22c1dcd23fe2b509484194ac9d4611"
$sonarHost = "http://localhost:9000"
$key = "Ofichinna"

dotnet sonarscanner begin /k:$key /d:sonar.host.url=$sonarHost /d:sonar.token=$token

dotnet build --no-incremental

dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"

dotnet sonarscanner end /d:sonar.token=$token