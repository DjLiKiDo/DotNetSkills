# Multi-stage Dockerfile for DotNetSkills API
# Uses official Microsoft images compatible with Azure App Service

#=====================================
# Build Stage
#=====================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file and project files for dependency resolution
COPY ["DotNetSkills.sln", "./"]
COPY ["src/DotNetSkills.API/DotNetSkills.API.csproj", "src/DotNetSkills.API/"]
COPY ["src/DotNetSkills.Application/DotNetSkills.Application.csproj", "src/DotNetSkills.Application/"]
COPY ["src/DotNetSkills.Domain/DotNetSkills.Domain.csproj", "src/DotNetSkills.Domain/"]
COPY ["src/DotNetSkills.Infrastructure/DotNetSkills.Infrastructure.csproj", "src/DotNetSkills.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/DotNetSkills.API/DotNetSkills.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/DotNetSkills.API"
# Restore again after copying the full source to ensure all analyzers/transitives are present
RUN dotnet restore "DotNetSkills.API.csproj"
RUN dotnet build "DotNetSkills.API.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

#=====================================
# Publish Stage
#=====================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DotNetSkills.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

#=====================================
# Runtime Stage
#=====================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Create logs directory with proper permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app/logs

# Copy published application
COPY --from=publish /app/publish .
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Configure ASP.NET Core
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Add labels for better container management
LABEL maintainer="DotNetSkills Team"
LABEL version="1.0"
LABEL description="DotNetSkills Clean Architecture API"

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "DotNetSkills.API.dll"]