$token = "sqp_6abcebcf2f064aef5cf4d8d34482578befd9d4f0"
$sonarHost = "http://localhost:9000"  
$key = "Ofichinna"  
  
dotnet sonarscanner begin /k:"$key" /d:sonar.host.url="$sonarHost" /d:sonar.token="$token" /d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml" /d:sonar.cpd.exclusions="**/Ofichina.Contracts/Requests/**/*.cs,**/Ofichina.Contracts/Responses/**/*.cs,**/Ofichina.Infrastructure/Migrations/**/*.cs,**/Ofichina.Api/Controllers/**/*.cs,**/Ofichina.Application/UseCases/**/*.cs,**/Ofichina.Application/Validators/**/*.cs"  
  
dotnet build --no-incremental  
dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"  
dotnet sonarscanner end /d:sonar.token="$token"