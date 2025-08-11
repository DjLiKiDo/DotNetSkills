# DotNetSkills DevOps Plan

## Objective
Set up local infrastructure for development, with Docker containers for the .NET API and the database, following DevOps best practices and Clean Architecture. Prepare the foundation for Staging and Production.


---


---

## Initial Docker Infrastructure Structure

The `Docker/` folder has been created at the root of the project to centralize everything related to containerization and orchestration.

**Initial contents:**
- `Dockerfile` (API)
- `docker-compose.yml`
- `.env.example` (environment variables)
- Migration and healthcheck scripts (future)
- Specific documentation (`README.md`)

> Edit and expand these files according to the needs of each environment and deployment.



---


---

## Preparation for Azure Deployment and CI/CD

### Base Images and Azure Compatibility
- Use official images compatible with Azure App Service and Azure Container Instances (e.g., `mcr.microsoft.com/dotnet/aspnet`, `mcr.microsoft.com/mssql/server`).
- Avoid hardcoding ports; use environment variables (`ASPNETCORE_URLS`).

### Environment Variables and Secrets
- Standardize the use of environment variables in Docker and Compose to facilitate integration with Azure Key Vault and GitHub Secrets.
- Document how to map local variables to secrets in GitHub Actions and Azure.

### Persistence and Volumes
- For local, use Docker volumes. For Azure, document the switch to Azure SQL Database and managed storage.

### Healthchecks and Readiness
- Add healthchecks in Docker Compose and the Dockerfile to facilitate monitoring in Azure.

### Migration Automation
- Prepare a script or entrypoint that runs migrations only if the environment variable (`RUN_MIGRATIONS=true`) is set, useful for CI/CD.

### Prepare for CI/CD
- Add a section on continuous integration:
	- Build Docker image in GitHub Actions.
	- Push to Azure Container Registry.
	- Deploy to Azure App Service/Container Instance.
	- Use Bicep/ARM templates for IaC.

### Multi-environment Configuration
- Document how to select the environment (`ASPNETCORE_ENVIRONMENT`) from GitHub Actions and Azure pipelines.

### Rollback and Troubleshooting Runbook
- Add steps for rollback and troubleshooting in cloud deployments.
- Create a multi-stage Dockerfile in `src/DotNetSkills.API/` to build and publish the API.
- Configure environment variables for Development, Staging, and Production using `appsettings.*.json` files.
- Expose the appropriate port (default 8080/5000).

### 2. Database in Container
- Use an official SQL Server image (or PostgreSQL if required).
- Configure credentials and database name per environment.
- Mount volume for local data persistence.

### 3. Orchestration with Docker Compose
- Create a `docker-compose.yml` file at the root of the project.
- Define services: `api` and `db`.
- Configure internal networks and dependencies (API depends on DB).
- Map ports and environment variables.
- Add healthchecks for both services.

### 4. Environment Configuration
- Use environment variables in Docker Compose to select the environment (`ASPNETCORE_ENVIRONMENT`).
- Allow override of configuration with `.env` files per environment.

### 5. Database Migrations
- Automate the execution of migrations when starting the API container (using `dotnet ef database update`).
- Document the process for local development.

### 6. Secrets Management
- For local, use environment variables and/or `.env` files (no sensitive secrets in the repo).
- Prepare future integration with Azure Key Vault for production.

### 7. Documentation and Runbook
- Document the process to start the local infrastructure (`README.md` or `docs/SECURITY-DEPLOYMENT.md`).
- Include commands for build, up, down, and troubleshooting.

---


## Actionable Tasks

- [x] Create multi-stage Dockerfile for the API in `src/DotNetSkills.API/` (completed)
- [ ] Create `docker-compose.yml` at the root with services for API and DB
- [ ] Configure `.env` files for Development, Staging, and Production
- [ ] Add healthchecks in Docker Compose for both services
- [ ] Automate database migrations on API startup
- [ ] Document the workflow in the README and/or docs
- [ ] Validate that the API and DB communicate correctly locally
- [ ] (Optional) Prepare scripts to clean and restart the local environment

---


## Notes
- Keep configuration decoupled per environment.
- Do not store secrets in the repository.
- Prepare the infrastructure to easily scale to Staging and Production.
