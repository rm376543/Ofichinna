$token = "sqp_ef2c39a9e025eedacf28e973c55e54716d81d6fe"
$sonarHost = "http://localhost:9000"
$key = "Ofichinna"

dotnet sonarscanner begin /k:$key /d:sonar.host.url=$sonarHost /d:sonar.token=$token

dotnet build --no-incremental

dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"

dotnet sonarscanner end /d:sonar.token=$token