# ===== ServiceDiscovery/Dockerfile =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ServiceDiscovery/ServiceDiscovery.csproj", "ServiceDiscovery/"]
RUN dotnet restore "ServiceDiscovery/ServiceDiscovery.csproj"
COPY . .
WORKDIR "/src/ServiceDiscovery"
RUN dotnet build "ServiceDiscovery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceDiscovery.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceDiscovery.dll"]
