FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2X.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Build.props backend/
COPY ServiceDefaults/ ./ServiceDefaults/
COPY backend/ ./backend/

# Build the Identity Service
RUN dotnet build backend/Domain/Identity/B2X.Identity.API.csproj -c Release /p:RunAnalyzers=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Domain/Identity/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7002

ENTRYPOINT ["dotnet", "B2Connect.Identity.API.dll"]
