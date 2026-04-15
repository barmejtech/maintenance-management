# ── Stage 1: Build ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage layer caching
COPY Maintenance-management.slnx ./
COPY Maintenance-management.domain/Maintenance-management.domain.csproj Maintenance-management.domain/
COPY Maintenance-management.application/Maintenance-management.application.csproj Maintenance-management.application/
COPY Maintenance-management.infrastructure/Maintenance-management.infrastructure.csproj Maintenance-management.infrastructure/
COPY Maintenance-management.api/Maintenance-management.api.csproj Maintenance-management.api/

RUN dotnet restore Maintenance-management.slnx

# Copy everything else and build
COPY . .
RUN dotnet publish Maintenance-management.api/Maintenance-management.api.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ───────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Default port for the API
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Maintenance-management.api.dll"]
