FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2X.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Build.props backend/
COPY ServiceDefaults/ ./ServiceDefaults/
COPY backend/ ./backend/

# Build the Tenancy Service
RUN dotnet build backend/Domain/Tenancy/B2X.Tenancy.API.csproj -c Release /p:RunAnalyzers=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Domain/Tenancy/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7003

ENTRYPOINT ["dotnet", "B2Connect.Tenancy.API.dll"]
