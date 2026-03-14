FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/PersonService.Api/*.csproj src/PersonService.Api/
COPY src/PersonService.Application/*.csproj src/PersonService.Application/
COPY src/PersonService.Domain/*.csproj src/PersonService.Domain/
COPY src/PersonService.Shared/*.csproj src/PersonService.Shared/
COPY src/PersonService.Infra.Data/*.csproj src/PersonService.Infra.Data/
COPY src/PersonService.Infra.Ioc/*.csproj src/PersonService.Infra.Ioc/

RUN dotnet restore src/PersonService.Api/PersonService.Api.csproj

COPY . .

WORKDIR /src/src/PersonService.Api

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet","PersonService.Api.dll"]

HEALTHCHECK --interval=10s --timeout=5s --start-period=10s --retries=5 \
 CMD curl -f http://localhost:8080/health || exit 1