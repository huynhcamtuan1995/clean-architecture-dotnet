#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/AppGateway/AppGateway.csproj", "samples/AppGateway/"]
RUN dotnet restore "samples/AppGateway/AppGateway.csproj"
COPY . .
WORKDIR "/src/samples/AppGateway"
RUN dotnet build "AppGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppGateway.dll"]