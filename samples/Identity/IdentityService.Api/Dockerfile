#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/Identity/IdentityService.Api/IdentityService.Api.csproj", "samples/Identity/IdentityService.Api/"]
COPY ["src/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/N8T.Infrastructure/"]
COPY ["src/N8T.Core/N8T.Core.csproj", "src/N8T.Core/"]
COPY ["src/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "src/N8T.Infrastructure.EfCore/"]
COPY ["samples/Identity/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "samples/Identity/IdentityService.Infrastructure/"]
COPY ["samples/Identity/IdentityService.AppCore/IdentityService.AppCore.csproj", "samples/Identity/IdentityService.AppCore/"]
COPY ["samples/DataContracts/AppContracts/AppContracts.csproj", "samples/DataContracts/AppContracts/"]
COPY ["samples/DataContracts/IntegrationEvents/IntegrationEvents.csproj", "samples/DataContracts/IntegrationEvents/"]
RUN dotnet restore "samples/Identity/IdentityService.Api/IdentityService.Api.csproj"
COPY . .
WORKDIR "/src/samples/Identity/IdentityService.Api"
RUN dotnet build "IdentityService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]