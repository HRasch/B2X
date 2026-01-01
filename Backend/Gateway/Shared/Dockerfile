# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["backend/Gateway/Shared/src/B2Connect.Shared.csproj", "backend/Gateway/Shared/src/"]
COPY ["backend/Domain/Support/src/B2Connect.Support.csproj", "backend/Domain/Support/src/"]
COPY ["backend/shared/B2Connect.Shared/B2Connect.Shared.csproj", "backend/shared/B2Connect.Shared/"]

RUN dotnet restore "backend/Gateway/Shared/src/B2Connect.Shared.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/backend/Gateway/Shared/src"
RUN dotnet build "B2Connect.Shared.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "B2Connect.Shared.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "B2Connect.Shared.dll"]