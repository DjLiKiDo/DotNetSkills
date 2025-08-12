# Deployment Guide

*Production deployment strategies and operational procedures for DotNetSkills*

## Deployment Overview

DotNetSkills supports multiple deployment strategies through containerization with Docker, designed for modern cloud-native environments.

### Supported Environments
- **Local Development** - Docker Compose for full-stack development
- **Staging/QA** - Container orchestration for testing
- **Production** - Cloud-native deployment with Azure/AWS/GCP

## Container Architecture

### Multi-Stage Docker Build
The API uses an optimized multi-stage Dockerfile:

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Compile and publish application

# Stage 2: Runtime  
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
# Lightweight runtime image
# Non-root user execution
# Health check integration
```

**Benefits**:
- **Optimized Size**: Runtime image ~200MB vs 1GB+ SDK image
- **Security**: Runs as non-root user
- **Performance**: Pre-compiled for faster startup
- **Health Monitoring**: Built-in health check endpoints

### Service Architecture
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Load Balancer │    │     API App     │    │   SQL Server    │
│   (nginx/ALB)   │───▶│   (Container)   │───▶│   (Container)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │      Redis      │
                       │   (Optional)    │
                       └─────────────────┘
```

## Environment Configuration

### Environment Variables

#### Core Application Settings
```bash
# Environment
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# Database
DOTNETSKILLS_Database__ConnectionString="Server=db;Database=DotNetSkills;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
DOTNETSKILLS_Database__CommandTimeout=30
DOTNETSKILLS_Database__MaxRetryCount=3

# Migration Control
RUN_MIGRATIONS=false  # Set to true only during deployments

# JWT Authentication
DOTNETSKILLS_Jwt__Enabled=true
DOTNETSKILLS_Jwt__Issuer=https://your-domain.com
DOTNETSKILLS_Jwt__Audience=dotnetskills-api
DOTNETSKILLS_Jwt__SigningKey=YourProductionSigningKey

# CORS
DOTNETSKILLS_Cors__PolicyName=ProductionCORS
DOTNETSKILLS_Cors__AllowedOrigins__0=https://your-frontend.com
DOTNETSKILLS_Cors__AllowCredentials=false

# Performance
DOTNETSKILLS_Performance__SlowRequestThresholdMs=1000
DOTNETSKILLS_Performance__EnableMetrics=true

# Security
DOTNETSKILLS_Swagger__Enabled=false  # Disable in production
```

#### Database Settings
```bash
# SQL Server
DB_SA_PASSWORD=YourSecurePassword123!
DB_NAME=DotNetSkills_Production

# Connection Pool
DB_MAX_POOL_SIZE=100
DB_CONNECTION_TIMEOUT=30
```

#### Caching (Optional)
```bash
# Redis Configuration
REDIS_PASSWORD=YourRedisPassword123!
REDIS_CONNECTION_STRING=redis:6379
REDIS_INSTANCE_NAME=DotNetSkills
```

### Configuration Hierarchy (Priority Order)
1. **Environment Variables** (`DOTNETSKILLS_*`)
2. **Azure Key Vault** (Production)
3. **Docker Secrets** (Swarm/Kubernetes)
4. **appsettings.Production.json**
5. **appsettings.json**

## Deployment Strategies

### 🐳 Docker Compose (Simple Deployment)

**Use Case**: Small deployments, staging environments

```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  api:
    image: dotnetskills:latest
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      RUN_MIGRATIONS: false
    ports:
      - "80:8080"
    depends_on:
      - db
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      retries: 3
    restart: unless-stopped

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: ${DB_SA_PASSWORD}
      ACCEPT_EULA: Y
    volumes:
      - sqldata:/var/opt/mssql
    restart: unless-stopped

volumes:
  sqldata:
```

**Deployment Commands**:
```bash
# Build and deploy
docker compose -f docker-compose.prod.yml up -d

# Update application
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d --no-deps api

# Health check
curl http://your-server/health
```

### ☁️ Azure Container Instances

**Use Case**: Serverless container deployment

```bash
# Create resource group
az group create --name dotnetskills-rg --location eastus

# Create container instance
az container create \
  --resource-group dotnetskills-rg \
  --name dotnetskills-api \
  --image dotnetskills:latest \
  --dns-name-label dotnetskills-api \
  --ports 8080 \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    RUN_MIGRATIONS=false \
  --secure-environment-variables \
    DOTNETSKILLS_Database__ConnectionString="Server=your-db;..." \
    DOTNETSKILLS_Jwt__SigningKey="your-signing-key"
```

### 🚢 Azure App Service (Container)

**Use Case**: Managed platform with built-in scaling

```bash
# Create App Service Plan
az appservice plan create \
  --name dotnetskills-plan \
  --resource-group dotnetskills-rg \
  --is-linux \
  --sku B1

# Create Web App
az webapp create \
  --resource-group dotnetskills-rg \
  --plan dotnetskills-plan \
  --name dotnetskills-api \
  --deployment-container-image-name dotnetskills:latest

# Configure environment variables
az webapp config appsettings set \
  --resource-group dotnetskills-rg \
  --name dotnetskills-api \
  --settings \
    ASPNETCORE_ENVIRONMENT=Production \
    WEBSITES_PORT=8080
```

### ⚓ Kubernetes Deployment

**Use Case**: Large-scale, orchestrated deployments

```yaml
# k8s-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: dotnetskills-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: dotnetskills-api
  template:
    metadata:
      labels:
        app: dotnetskills-api
    spec:
      containers:
      - name: api
        image: dotnetskills:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: RUN_MIGRATIONS
          value: "false"
        envFrom:
        - secretRef:
            name: dotnetskills-secrets
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: dotnetskills-service
spec:
  selector:
    app: dotnetskills-api
  ports:
  - port: 80
    targetPort: 8080
  type: LoadBalancer
```

**Deployment Commands**:
```bash
# Apply configuration
kubectl apply -f k8s-deployment.yaml

# Check status
kubectl get pods
kubectl get services

# Scale deployment
kubectl scale deployment dotnetskills-api --replicas=5
```

## Database Migration Strategy

### Migration Deployment Patterns

#### 🔄 Automated Migrations (Recommended for Development/Staging)
```bash
# Enable automatic migrations on startup
RUN_MIGRATIONS=true

# Application will:
# 1. Check database connection
# 2. Apply pending migrations
# 3. Log migration status
# 4. Start normally or fail fast
```

#### 🎯 Manual Migrations (Recommended for Production)
```bash
# 1. Generate migration script
dotnet ef migrations script --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API --output migration.sql

# 2. Review script for safety
# 3. Apply during maintenance window
# 4. Deploy application with RUN_MIGRATIONS=false
```

#### 🚀 Blue-Green Deployment with Migrations
```bash
# 1. Deploy new version to "green" environment
# 2. Run migrations against shared database
# 3. Test green environment thoroughly
# 4. Switch traffic from blue to green
# 5. Decommission blue environment
```

### Migration Best Practices
- **Always backup before migrations**
- **Test migrations on staging first**
- **Use transaction-safe migrations when possible**
- **Plan rollback strategy for each migration**
- **Monitor performance impact of large migrations**

## Security Configuration

### Production Security Checklist

#### ✅ Application Security
- [ ] JWT signing key is cryptographically secure (256+ bits)
- [ ] HTTPS enforced (disable HTTP redirect in container, handle at load balancer)
- [ ] CORS configured for specific origins only
- [ ] Swagger disabled (`DOTNETSKILLS_Swagger__Enabled=false`)
- [ ] Detailed error messages disabled
- [ ] Sensitive data logging disabled

#### ✅ Infrastructure Security
- [ ] Run containers as non-root user
- [ ] Use minimal base images (distroless when possible)
- [ ] Secrets stored in secure key management (Azure Key Vault, Kubernetes Secrets)
- [ ] Network policies restrict unnecessary communication
- [ ] Regular security updates applied to base images

#### ✅ Database Security
- [ ] Strong SA password or managed identity authentication
- [ ] Database firewall configured
- [ ] Encryption at rest enabled
- [ ] Backup encryption enabled
- [ ] Connection string stored securely

### Secrets Management

#### Azure Key Vault Integration
```csharp
// Configured automatically in production
// src/DotNetSkills.API/Program.cs
builder.Configuration.AddAzureKeyVaultIfProduction(builder.Environment);
```

#### Kubernetes Secrets
```bash
# Create secret
kubectl create secret generic dotnetskills-secrets \
  --from-literal=database-connection="Server=..." \
  --from-literal=jwt-signing-key="..."

# Reference in deployment
envFrom:
- secretRef:
    name: dotnetskills-secrets
```

## Monitoring and Observability

### Health Checks
```bash
# Application health endpoint
curl http://your-app/health

# Response indicates:
# - Application startup status
# - Database connectivity
# - Overall health score
```

### Logging Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "/app/logs/dotnetskills-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

### Performance Monitoring
```bash
# Key metrics to monitor:
# - Request latency (P50, P95, P99)
# - Error rate
# - Database connection pool utilization
# - Memory and CPU usage
# - Health check response time
```

### Correlation ID Tracking
All requests include correlation IDs for distributed tracing:
```http
# Request
GET /api/v1/users
X-Correlation-Id: abc123

# Response  
HTTP/1.1 200 OK
X-Correlation-Id: abc123
```

## Backup and Recovery

### Database Backup Strategy
```bash
# Automated backups (Azure SQL Database)
# - Point-in-time restore: 7-35 days
# - Long-term retention: Up to 10 years
# - Geo-redundant backups for disaster recovery

# Manual backup
az sql db export \
  --resource-group dotnetskills-rg \
  --server dotnetskills-sql \
  --name DotNetSkills \
  --admin-user sqladmin \
  --storage-uri https://storage.blob.core.windows.net/backups/backup.bacpac
```

### Application State
- **Stateless Design**: Application maintains no local state
- **Configuration**: Externalized via environment variables
- **Secrets**: Stored in secure key management systems
- **Logs**: Persisted to external logging systems

## Scaling Strategies

### Horizontal Scaling
```bash
# Docker Swarm
docker service scale dotnetskills-api=5

# Kubernetes
kubectl scale deployment dotnetskills-api --replicas=5

# Azure App Service
az appservice plan update --name dotnetskills-plan --number-of-workers 3
```

### Vertical Scaling
```bash
# Increase container resources
docker update --memory=2g --cpus=2 dotnetskills-api

# Azure App Service plan upgrade
az appservice plan update --name dotnetskills-plan --sku P1V3
```

### Database Scaling
- **Read Replicas**: For read-heavy workloads
- **Connection Pooling**: Optimize connection utilization
- **Caching**: Redis for frequently accessed data
- **Partitioning**: For very large datasets

## Deployment Pipelines

### GitHub Actions Example
```yaml
name: Deploy to Production

on:
  release:
    types: [published]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Build Docker Image
      run: docker build -t dotnetskills:${{ github.sha }} .
    
    - name: Push to Registry
      run: |
        docker tag dotnetskills:${{ github.sha }} registry/dotnetskills:${{ github.sha }}
        docker push registry/dotnetskills:${{ github.sha }}
    
    - name: Deploy to Production
      run: |
        # Update deployment with new image
        kubectl set image deployment/dotnetskills-api api=registry/dotnetskills:${{ github.sha }}
        kubectl rollout status deployment/dotnetskills-api
```

### Azure DevOps Pipeline
```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

stages:
- stage: Build
  jobs:
  - job: BuildAndTest
    steps:
    - task: DockerBuild@2
      inputs:
        containerRegistry: 'AzureContainerRegistry'
        repository: 'dotnetskills'
        tags: |
          $(Build.BuildNumber)
          latest

- stage: Deploy
  dependsOn: Build
  jobs:
  - deployment: DeployToProduction
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebAppContainer@1
            inputs:
              appName: 'dotnetskills-api'
              imageName: 'registry.azurecr.io/dotnetskills:$(Build.BuildNumber)'
```

## Troubleshooting Deployment Issues

### Common Issues

| Issue | Symptoms | Solution |
|-------|----------|----------|
| Health check fails | Container marked unhealthy | Check `/health` endpoint, database connectivity |
| Migrations fail | Application won't start | Verify connection string, database permissions |
| High memory usage | Container OOMKilled | Increase memory limits, check for memory leaks |
| Slow startup | Long readiness probe delays | Optimize startup code, increase probe timeouts |
| Configuration errors | App fails to start | Validate environment variables, check logs |

### Diagnostic Commands
```bash
# Container logs
docker logs dotnetskills-api

# Kubernetes troubleshooting
kubectl describe pod dotnetskills-api-xxx
kubectl logs dotnetskills-api-xxx

# Health check testing
curl -v http://your-app/health

# Database connectivity test
telnet db-server 1433
```

## Rollback Procedures

### Quick Rollback
```bash
# Docker Compose
docker compose -f docker-compose.prod.yml pull dotnetskills:previous-version
docker compose -f docker-compose.prod.yml up -d --no-deps api

# Kubernetes
kubectl rollout undo deployment/dotnetskills-api

# Azure App Service
az webapp deployment slot swap --name dotnetskills-api --resource-group dotnetskills-rg --slot staging
```

### Database Rollback
```bash
# Point-in-time restore (Azure SQL)
az sql db restore \
  --dest-name DotNetSkills-Restored \
  --edition Standard \
  --name DotNetSkills \
  --resource-group dotnetskills-rg \
  --server dotnetskills-sql \
  --time "2024-01-15T10:30:00"
```

---

**🚀 Deployment Success Criteria**:
- Health checks pass consistently
- Application responds within acceptable latency
- Database migrations completed successfully
- Monitoring and logging operational
- Security configurations verified
- Rollback procedures tested and documented