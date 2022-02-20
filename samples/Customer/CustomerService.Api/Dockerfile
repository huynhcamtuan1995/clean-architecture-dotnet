#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/Customer/CustomerService.Api/CustomerService.Api.csproj", "samples/Customer/CustomerService.Api/"]
COPY ["src/N8T.Infrastructure/N8T.Infrastructure.csproj", "src/N8T.Infrastructure/"]
COPY ["src/N8T.Core/N8T.Core.csproj", "src/N8T.Core/"]
COPY ["samples/Customer/CustomerService.AppCore/CustomerService.AppCore.csproj", "samples/Customer/CustomerService.AppCore/"]
COPY ["samples/DataContracts/AppContracts/AppContracts.csproj", "samples/DataContracts/AppContracts/"]
COPY ["samples/DataContracts/IntegrationEvents/IntegrationEvents.csproj", "samples/DataContracts/IntegrationEvents/"]
COPY ["src/N8T.Infrastructure.OTel/N8T.Infrastructure.OTel.csproj", "src/N8T.Infrastructure.OTel/"]
COPY ["src/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "src/N8T.Infrastructure.EfCore/"]
COPY ["samples/Customer/CustomerService.Infrastructure/CustomerService.Infrastructure.csproj", "samples/Customer/CustomerService.Infrastructure/"]
RUN dotnet restore "samples/Customer/CustomerService.Api/CustomerService.Api.csproj"
COPY . .
WORKDIR "/src/samples/Customer/CustomerService.Api"
RUN dotnet build "CustomerService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustomerService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerService.Api.dll"]