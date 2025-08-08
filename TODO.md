# Domain Layer TODOs

[x] Review entities
[x] Folder structure matching Domain looking for better organization and namespacing
[x] Implement extensions for enums
[x] Review BusinesRules vs ValidationConstants vs ValidationMessages
[x] Work on technical debt `DomainTechnicalDebt.md`
[x] We have BoundedContextUsings in UserManagement and TeamCollaboration but no in ProjectManagement nor TaskExecution. Review the best approach and consolidate the most appropiate implementation
[ ] Domain layer specific documentation

# API Layer TODOs

[ ] BaseEndpoint.cs
[ ] Review ExceptionHandlingMiddleware.cs
[ ] API layer specific documentation

# Application Layer TODOs

[ ] Application layer specific documentation

# Infrastructure Layer TODOs

[ ] Migrations
[ ] there are some warnings about shadow properties for TeamMember foreign keys. This is a known issue with EF Core when using strongly-typed IDs
[ ] Database Creation: Run dotnet ef database update to create the database schema
[ ] Production Deployment: Configure connection strings and deploy
[ ] Infrastructure layer specific documentation
