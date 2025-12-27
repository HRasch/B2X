FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2Connect.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Packages.props backend/
COPY backend/Directory.Build.props backend/
COPY backend/ ./backend/

# Build the Tenancy Service
RUN dotnet build backend/Domain/Tenancy/B2Connect.Tenancy.API.csproj -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Domain/Tenancy/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7003

ENTRYPOINT ["dotnet", "B2Connect.Tenancy.API.dll"]
