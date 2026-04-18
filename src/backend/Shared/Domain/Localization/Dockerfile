FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2X.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Build.props backend/
COPY ServiceDefaults/ ./ServiceDefaults/
COPY backend/ ./backend/

# Build the Localization Service
RUN dotnet build backend/Domain/Localization/B2X.Localization.API.csproj -c Release /p:RunAnalyzers=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Domain/Localization/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7004

ENTRYPOINT ["dotnet", "B2Connect.Localization.API.dll"]
