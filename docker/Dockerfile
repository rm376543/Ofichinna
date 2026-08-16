FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["src/Ofichina.Api/Ofichina.Api.csproj", "src/Ofichina.Api/"]
COPY ["src/Ofichina.Application/Ofichina.Application.csproj", "src/Ofichina.Application/"]
COPY ["src/Ofichina.Authentication/Ofichina.Authentication.csproj", "src/Ofichina.Authentication/"]
COPY ["src/Ofichina.Bootstrap/Ofichina.Bootstrap.csproj", "src/Ofichina.Bootstrap/"]
COPY ["src/Ofichina.Contracts/Ofichina.Contracts.csproj", "src/Ofichina.Contracts/"]
COPY ["src/Ofichina.Domain/Ofichina.Domain.csproj", "src/Ofichina.Domain/"]
COPY ["src/Ofichina.Infrastructure/Ofichina.Infrastructure.csproj", "src/Ofichina.Infrastructure/"]

RUN dotnet restore "src/Ofichina.Api/Ofichina.Api.csproj"

COPY . .

WORKDIR "/src/src/Ofichina.Api"

RUN dotnet publish "Ofichina.Api.csproj" \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Ofichina.Api.dll"]