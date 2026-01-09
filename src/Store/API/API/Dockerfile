FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2X.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Build.props backend/
COPY ../../../Hosting/ServiceDefaults/ ./ServiceDefaults/
COPY backend/ ./backend/

# Build the Store Gateway
RUN dotnet build backend/Gateway/Store/API/B2X.Store.csproj -c Release /p:RunAnalyzers=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Gateway/Store/API/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 8000

ENTRYPOINT ["dotnet", "B2X.Store.dll"]
