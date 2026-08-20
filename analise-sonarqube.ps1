$token = "sqp_cf8689b77d22c1dcd23fe2b509484194ac9d4611"
$sonarHost = "http://localhost:9000"
$key = "Ofichinna"

dotnet sonarscanner begin /k:$key /d:sonar.host.url=$sonarHost /d:sonar.token=$token /d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml" /d:sonar.cpd.exclusions=**/Ofichina.Contracts/Requests/**/*.cs,**/Ofichina.Contracts/Responses/**/*.cs,**/Ofichina.Infrastructure/Migrations/**/*.cs,**/Ofichina.Api/Controllers/**/*.cs,**/Ofichina.Application/UseCases/**/*.cs,**/Ofichina.Application/Validators/**/*.cs

dotnet build --no-incremental

dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"

dotnet sonarscanner end /d:sonar.token=$token