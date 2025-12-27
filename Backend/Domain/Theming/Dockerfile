FROM mcr.microsoft.com/dotnet/sdk:10.0 as build
WORKDIR /src

# Copy solution and project files
COPY B2Connect.slnx .
COPY Directory.Packages.props .
COPY backend/Directory.Packages.props backend/
COPY backend/Directory.Build.props backend/
COPY backend/ ./backend/

# Build the Theming Service
RUN dotnet build backend/Domain/Theming/B2Connect.Theming.API.csproj -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /src/backend/Domain/Theming/bin/Release/net10.0 .

ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 7008

ENTRYPOINT ["dotnet", "B2Connect.Theming.API.dll"]
