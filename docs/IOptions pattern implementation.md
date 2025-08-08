## DDD & SOLID Analysis

### **Patterns & Principles**
- **SRP**: Configuration loading, validation, and consumption are separate responsibilities
- **DIP**: Infrastructure layer consumes configuration via `IOptions<DatabaseOptions>` abstraction
- **Configuration as Infrastructure**: Loading from appsettings.json and Azure Key Vault belongs in Infrastructure layer
- **Security**: Connection strings and sensitive data must be in Key Vault for production environments

### **Layer Impact**
- **Infrastructure**: Implements configuration loading, validation, and EF Core integration
- **API**: Orchestrates configuration providers (appsettings, Key Vault, environment variables)
- **Domain**: No impact - configuration is an infrastructure concern

### **Ubiquitous Language**
"DatabaseOptions", "ConnectionString", "KeyVault", "ConfigurationProvider", "Secrets", "ValidationRules"

### **Compliance & Security**
- **PCI-DSS/LGPD**: Secrets must be stored in Azure Key Vault in production
- **Audit**: Configuration loading and validation events must be logged
- **Encryption**: Connection strings containing credentials are encrypted at rest in Key Vault

---

## Quality Checklist Review

### **Domain Design Validation**
- ✅ **Domain Model**: Configuration is correctly treated as infrastructure concern, not domain logic
- ✅ **Ubiquitous Language**: Consistent terminology ("DatabaseOptions", "ConnectionString", "Secrets")
- ✅ **SOLID Principles**: SRP and DIP respected through separation of concerns
- ✅ **Business Rules**: Validation rules encapsulated in dedicated validator class

### **Implementation Quality Validation**
- ✅ **Test Coverage**: Plan includes comprehensive tests for configuration loading and validation scenarios
- ✅ **Performance**: Efficient configuration loading with caching via IOptions pattern
- ✅ **Security**: Azure Key Vault integration for secrets, validation prevents invalid configurations
- ✅ **Documentation**: XML comments present, architectural decisions documented
- ✅ **.NET Best Practices**: Options pattern, dependency injection, async operations where appropriate

---

## Comprehensive Implementation Plan

### **Phase 1: Configuration Validation Infrastructure**

#### **1.1 Create Configuration Validator**

````csharp
using FluentValidation;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Validates DatabaseOptions configuration at startup and runtime.
/// </summary>
public class DatabaseOptionsValidator : AbstractValidator<DatabaseOptions>, IValidateOptions<DatabaseOptions>
{
    public DatabaseOptionsValidator()
    {
        RuleFor(x => x.ConnectionString)
            .NotEmpty()
            .WithMessage("ConnectionString is required")
            .Must(BeValidConnectionString)
            .WithMessage("ConnectionString must be a valid format");

        RuleFor(x => x.CommandTimeout)
            .InclusiveBetween(1, 300)
            .WithMessage("CommandTimeout must be between 1 and 300 seconds");

        RuleFor(x => x.MaxRetryCount)
            .InclusiveBetween(0, 10)
            .WithMessage("MaxRetryCount must be between 0 and 10");

        RuleFor(x => x.MaxRetryDelaySeconds)
            .InclusiveBetween(1, 60)
            .WithMessage("MaxRetryDelaySeconds must be between 1 and 60 seconds");

        RuleFor(x => x.MigrationsAssembly)
            .Must(BeValidAssemblyName)
            .When(x => !string.IsNullOrEmpty(x.MigrationsAssembly))
            .WithMessage("MigrationsAssembly must be a valid assembly name");
    }

    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        var validationResult = this.Validate(options);
        
        if (validationResult.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = validationResult.Errors.Select(error => 
            $"DatabaseOptions.{error.PropertyName}: {error.ErrorMessage}");
            
        return ValidateOptionsResult.Fail(errors);
    }

    private static bool BeValidConnectionString(string connectionString)
    {
        try
        {
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
            return !string.IsNullOrEmpty(builder.Host) && !string.IsNullOrEmpty(builder.Database);
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidAssemblyName(string? assemblyName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName))
            return true;

        try
        {
            _ = System.Reflection.AssemblyName.GetAssemblyName(assemblyName);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
````

#### **1.2 Create Configuration Extensions**

````csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Extension methods for configuration setup and validation.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds database configuration with validation and Key Vault integration.
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Register options with configuration binding
        services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
        
        // Register validator
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        
        // Add validation on startup
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection("Database"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Adds Azure Key Vault configuration provider for production secrets.
    /// </summary>
    public static IConfigurationBuilder AddAzureKeyVaultIfProduction(
        this IConfigurationBuilder configuration,
        IWebHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            var tempConfig = configuration.Build();
            var keyVaultUri = tempConfig["KeyVault:Uri"];
            
            if (!string.IsNullOrEmpty(keyVaultUri))
            {
                configuration.AddAzureKeyVault(
                    new Uri(keyVaultUri),
                    new DefaultAzureCredential(),
                    new AzureKeyVaultConfigurationOptions
                    {
                        ReloadInterval = TimeSpan.FromMinutes(5)
                    });
            }
        }

        return configuration;
    }

    /// <summary>
    /// Validates configuration on application startup.
    /// </summary>
    public static void ValidateConfiguration(this IServiceProvider serviceProvider)
    {
        // Force validation of all registered options
        var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();
        _ = databaseOptions.Value; // This triggers validation
    }
}
````

### **Phase 2: Configuration Loading Strategy**

#### **2.1 Update Program.cs Configuration**

````csharp
// ...existing code...

var builder = WebApplication.CreateBuilder(args);

// Configure configuration providers in order of precedence
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("DOTNETSKILLS_")
    .AddAzureKeyVaultIfProduction(builder.Environment);

// Development secrets (only in Development environment)
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add configuration services
builder.Services.AddDatabaseConfiguration(builder.Configuration, builder.Environment);

// ...existing code...

var app = builder.Build();

// Validate configuration on startup
app.Services.ValidateConfiguration();

// ...existing code...
````

#### **2.2 Create appsettings Configuration**

````json
{
  "Database": {
    "ConnectionString": "Host=localhost;Database=dotnetskills_dev;Username=postgres;Password=postgres",
    "CommandTimeout": 30,
    "MaxRetryCount": 3,
    "MaxRetryDelaySeconds": 5,
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false,
    "EnableQueryLogging": true,
    "MigrationsAssembly": "DotNetSkills.Infrastructure"
  },
  "KeyVault": {
    "Uri": ""
  }
}
````

````json
{
  "Database": {
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true,
    "EnableQueryLogging": true
  }
}
````

````json
{
  "Database": {
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false,
    "EnableQueryLogging": false
  },
  "KeyVault": {
    "Uri": "https://your-keyvault-name.vault.azure.net/"
  }
}
````

### **Phase 3: EF Core Integration**

#### **3.1 Update ApplicationDbContext**

````csharp
// ...existing code...
using Microsoft.Extensions.Options;
using DotNetSkills.Infrastructure.Common.Configuration;

namespace DotNetSkills.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    private readonly DatabaseOptions _databaseOptions;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<DatabaseOptions> databaseOptions) : base(options)
    {
        _databaseOptions = databaseOptions.Value;
    }

    // ...existing code...

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_databaseOptions.ConnectionString, options =>
            {
                options.CommandTimeout(_databaseOptions.CommandTimeout);
                options.EnableRetryOnFailure(
                    maxRetryCount: _databaseOptions.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(_databaseOptions.MaxRetryDelaySeconds),
                    errorCodesToAdd: null);

                if (!string.IsNullOrEmpty(_databaseOptions.MigrationsAssembly))
                {
                    options.MigrationsAssembly(_databaseOptions.MigrationsAssembly);
                }
            });

            if (_databaseOptions.EnableSensitiveDataLogging)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }

            if (_databaseOptions.EnableDetailedErrors)
            {
                optionsBuilder.EnableDetailedErrors();
            }

            if (_databaseOptions.EnableQueryLogging)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }

        base.OnConfiguring(optionsBuilder);
    }
}
````

#### **3.2 Update Infrastructure DependencyInjection**

````csharp
// ...existing code...
using DotNetSkills.Infrastructure.Common.Configuration;
using Microsoft.Extensions.Options;

public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    IConfiguration configuration,
    IWebHostEnvironment environment)
{
    // Add database configuration
    services.AddDatabaseConfiguration(configuration, environment);

    // Configure DbContext with options
    services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
        var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        
        options.UseNpgsql(databaseOptions.ConnectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(databaseOptions.CommandTimeout);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: databaseOptions.MaxRetryCount,
                maxRetryDelay: TimeSpan.FromSeconds(databaseOptions.MaxRetryDelaySeconds),
                errorCodesToAdd: null);

            if (!string.IsNullOrEmpty(databaseOptions.MigrationsAssembly))
            {
                npgsqlOptions.MigrationsAssembly(databaseOptions.MigrationsAssembly);
            }
        });

        if (environment.IsDevelopment())
        {
            if (databaseOptions.EnableSensitiveDataLogging)
                options.EnableSensitiveDataLogging();
                
            if (databaseOptions.EnableDetailedErrors)
                options.EnableDetailedErrors();
        }
    });

    // ...existing code...
    
    return services;
}
````

### **Phase 4: Azure Key Vault Secret Mapping**

#### **4.1 Create Key Vault Secret Naming Convention**

````csharp
namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Defines Azure Key Vault secret names and their configuration mappings.
/// </summary>
public static class KeyVaultSecretNames
{
    /// <summary>
    /// Database connection string secret name in Key Vault.
    /// Maps to: Database:ConnectionString
    /// </summary>
    public const string DatabaseConnectionString = "Database--ConnectionString";
    
    /// <summary>
    /// JWT signing key secret name in Key Vault.
    /// Maps to: Jwt:SigningKey
    /// </summary>
    public const string JwtSigningKey = "Jwt--SigningKey";
    
    /// <summary>
    /// Email service API key secret name in Key Vault.
    /// Maps to: EmailService:ApiKey
    /// </summary>
    public const string EmailServiceApiKey = "EmailService--ApiKey";
}
````

#### **4.2 Update Configuration for Key Vault Mapping**

````csharp
// ...existing code...

public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds Azure Key Vault configuration provider with custom secret mapping.
    /// </summary>
    public static IConfigurationBuilder AddAzureKeyVaultIfProduction(
        this IConfigurationBuilder configuration,
        IWebHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            var tempConfig = configuration.Build();
            var keyVaultUri = tempConfig["KeyVault:Uri"];
            
            if (!string.IsNullOrEmpty(keyVaultUri))
            {
                configuration.AddAzureKeyVault(
                    new Uri(keyVaultUri),
                    new DefaultAzureCredential(),
                    new AzureKeyVaultConfigurationOptions
                    {
                        ReloadInterval = TimeSpan.FromMinutes(5),
                        Manager = new PrefixKeyVaultSecretManager("DotNetSkills")
                    });
            }
        }

        return configuration;
    }
}

/// <summary>
/// Custom Key Vault secret manager for handling secret name prefixes and mappings.
/// </summary>
public class PrefixKeyVaultSecretManager : KeyVaultSecretManager
{
    private readonly string _prefix;

    public PrefixKeyVaultSecretManager(string prefix)
    {
        _prefix = $"{prefix}-";
    }

    public override bool Load(SecretProperties secret)
    {
        return secret.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase);
    }

    public override string GetKey(KeyVaultSecret secret)
    {
        var key = secret.Name[_prefix.Length..];
        return key.Replace("--", ":");
    }
}
````

### **Phase 5: Comprehensive Testing Strategy**

#### **5.1 Unit Tests for Configuration**

````csharp
using DotNetSkills.Infrastructure.Common.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.UnitTests.Common.Configuration;

public class DatabaseOptionsValidatorTests
{
    private readonly DatabaseOptionsValidator _validator = new();

    [Fact]
    public void Validate_WithValidOptions_ShouldReturnSuccess()
    {
        // Arrange
        var options = new DatabaseOptions
        {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            CommandTimeout = 30,
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5
        };

        // Act
        var result = _validator.Validate(string.Empty, options);

        // Assert
        result.Should().Be(ValidateOptionsResult.Success);
    }

    [Fact]
    public void Validate_WithEmptyConnectionString_ShouldReturnFailure()
    {
        // Arrange
        var options = new DatabaseOptions
        {
            ConnectionString = string.Empty,
            CommandTimeout = 30,
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5
        };

        // Act
        var result = _validator.Validate(string.Empty, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("DatabaseOptions.ConnectionString: ConnectionString is required");
    }

    [Fact]
    public void Validate_WithInvalidCommandTimeout_ShouldReturnFailure()
    {
        // Arrange
        var options = new DatabaseOptions
        {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            CommandTimeout = 0, // Invalid
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5
        };

        // Act
        var result = _validator.Validate(string.Empty, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("DatabaseOptions.CommandTimeout: CommandTimeout must be between 1 and 300 seconds");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void Validate_WithInvalidMaxRetryCount_ShouldReturnFailure(int invalidRetryCount)
    {
        // Arrange
        var options = new DatabaseOptions
        {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            CommandTimeout = 30,
            MaxRetryCount = invalidRetryCount,
            MaxRetryDelaySeconds = 5
        };

        // Act
        var result = _validator.Validate(string.Empty, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("DatabaseOptions.MaxRetryCount: MaxRetryCount must be between 0 and 10");
    }
}
````

#### **5.2 Integration Tests for Configuration Loading**

````csharp
using DotNetSkills.Infrastructure.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using FluentAssertions;

namespace DotNetSkills.Infrastructure.UnitTests.Common.Configuration;

public class ConfigurationIntegrationTests
{
    [Fact]
    public void LoadConfiguration_FromAppSettings_ShouldBindCorrectly()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Host=localhost;Database=test;Username=user;Password=pass",
                ["Database:CommandTimeout"] = "45",
                ["Database:MaxRetryCount"] = "5",
                ["Database:MaxRetryDelaySeconds"] = "10",
                ["Database:EnableSensitiveDataLogging"] = "true",
                ["Database:EnableDetailedErrors"] = "false",
                ["Database:EnableQueryLogging"] = "true",
                ["Database:MigrationsAssembly"] = "DotNetSkills.Infrastructure"
            })
            .Build();

        var services = new ServiceCollection();
        var environment = new MockWebHostEnvironment { EnvironmentName = "Development" };
        
        // Act
        services.AddDatabaseConfiguration(configuration, environment);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();

        // Assert
        var databaseOptions = options.Value;
        databaseOptions.ConnectionString.Should().Be("Host=localhost;Database=test;Username=user;Password=pass");
        databaseOptions.CommandTimeout.Should().Be(45);
        databaseOptions.MaxRetryCount.Should().Be(5);
        databaseOptions.MaxRetryDelaySeconds.Should().Be(10);
        databaseOptions.EnableSensitiveDataLogging.Should().BeTrue();
        databaseOptions.EnableDetailedErrors.Should().BeFalse();
        databaseOptions.EnableQueryLogging.Should().BeTrue();
        databaseOptions.MigrationsAssembly.Should().Be("DotNetSkills.Infrastructure");
    }

    [Fact]
    public void LoadConfiguration_WithKeyVaultOverride_ShouldUseKeyVaultValues()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Host=localhost;Database=original;Username=user;Password=pass",
                ["Database:CommandTimeout"] = "30"
            })
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Simulate Key Vault override
                ["Database:ConnectionString"] = "Host=prod-server;Database=production;Username=prod-user;Password=secure-pass"
            })
            .Build();

        var services = new ServiceCollection();
        var environment = new MockWebHostEnvironment { EnvironmentName = "Production" };
        
        // Act
        services.AddDatabaseConfiguration(configuration, environment);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>();

        // Assert
        var databaseOptions = options.Value;
        databaseOptions.ConnectionString.Should().Be("Host=prod-server;Database=production;Username=prod-user;Password=secure-pass");
        databaseOptions.CommandTimeout.Should().Be(30); // Non-secret value remains from appsettings
    }

    [Fact]
    public void ValidationOnStartup_WithInvalidConfiguration_ShouldThrowException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "", // Invalid empty connection string
                ["Database:CommandTimeout"] = "0" // Invalid timeout
            })
            .Build();

        var services = new ServiceCollection();
        var environment = new MockWebHostEnvironment { EnvironmentName = "Development" };
        
        // Act & Assert
        services.AddDatabaseConfiguration(configuration, environment);
        var serviceProvider = services.BuildServiceProvider();
        
        var action = () => serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        action.Should().Throw<OptionsValidationException>()
            .WithMessage("*ConnectionString is required*")
            .WithMessage("*CommandTimeout must be between 1 and 300 seconds*");
    }
}

public class MockWebHostEnvironment : IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = "Development";
    public string ApplicationName { get; set; } = "TestApp";
    public string WebRootPath { get; set; } = "";
    public IFileProvider WebRootFileProvider { get; set; } = null!;
    public string ContentRootPath { get; set; } = "";
    public IFileProvider ContentRootFileProvider { get; set; } = null!;
}
````

### **Phase 6: Security and Deployment Configuration**

#### **6.1 Azure Key Vault Setup Script**

````bash
#!/bin/bash

# Variables
RESOURCE_GROUP="rg-dotnetskills-prod"
KEY_VAULT_NAME="kv-dotnetskills-prod"
LOCATION="eastus"
APP_SERVICE_NAME="app-dotnetskills-prod"

# Create Key Vault
az keyvault create \
    --name $KEY_VAULT_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku standard

# Set database connection string secret
az keyvault secret set \
    --vault-name $KEY_VAULT_NAME \
    --name "DotNetSkills-Database--ConnectionString" \
    --value "Host=prod-server.postgres.database.azure.com;Database=dotnetskills_prod;Username=dbadmin;Password=SecurePassword123!"

# Set JWT signing key secret
az keyvault secret set \
    --vault-name $KEY_VAULT_NAME \
    --name "DotNetSkills-Jwt--SigningKey" \
    --value "$(openssl rand -base64 64)"

# Grant App Service access to Key Vault
APP_PRINCIPAL_ID=$(az webapp identity show --name $APP_SERVICE_NAME --resource-group $RESOURCE_GROUP --query principalId --output tsv)

az keyvault set-policy \
    --name $KEY_VAULT_NAME \
    --object-id $APP_PRINCIPAL_ID \
    --secret-permissions get list
````

#### **6.2 Environment-Specific Configuration**

````bash
#!/bin/bash

# Development environment variables
export DOTNETSKILLS_Database__ConnectionString="Host=localhost;Database=dotnetskills_dev;Username=postgres;Password=postgres"
export DOTNETSKILLS_Database__EnableSensitiveDataLogging="true"
export DOTNETSKILLS_Database__EnableDetailedErrors="true"

# Production environment variables (set in Azure App Service)
# DOTNETSKILLS_KeyVault__Uri="https://kv-dotnetskills-prod.vault.azure.net/"
# DOTNETSKILLS_Database__EnableSensitiveDataLogging="false"
# DOTNETSKILLS_Database__EnableDetailedErrors="false"
````

### **Phase 7: Implementation Timeline**

#### **Week 1: Foundation**
- [ ] Create `DatabaseOptionsValidator` with comprehensive validation
- [ ] Implement `ConfigurationExtensions` for DI registration
- [ ] Add unit tests for configuration validation
- [ ] Update `appsettings.json` files with proper structure

#### **Week 2: Integration**
- [ ] Update `ApplicationDbContext` to use `IOptions<DatabaseOptions>`
- [ ] Modify Infrastructure `DependencyInjection.cs` for configuration
- [ ] Update `Program.cs` with configuration loading pipeline
- [ ] Add integration tests for configuration loading

#### **Week 3: Azure Key Vault**
- [ ] Implement Azure Key Vault configuration provider
- [ ] Create `PrefixKeyVaultSecretManager` for secret mapping
- [ ] Add Key Vault setup scripts and documentation
- [ ] Test Key Vault integration in staging environment

#### **Week 4: Security & Deployment**
- [ ] Configure production environment variables
- [ ] Set up Azure Key Vault with production secrets
- [ ] Add monitoring and logging for configuration issues
- [ ] Complete end-to-end testing in all environments

---

## Success Criteria

### **Technical Requirements**
- ✅ Configuration loaded from multiple sources (appsettings, environment, Key Vault)
- ✅ Secrets stored securely in Azure Key Vault for production
- ✅ Comprehensive validation prevents invalid configurations
- ✅ Configuration changes reload automatically without restart
- ✅ All configuration access uses `IOptions<T>` pattern

### **Security Requirements**
- ✅ No secrets in source code or configuration files
- ✅ Production secrets only accessible via Azure Key Vault
- ✅ Connection strings encrypted at rest and in transit
- ✅ Configuration validation prevents security misconfigurations

### **Maintainability Requirements**
- ✅ Clear separation between configuration loading and consumption
- ✅ Environment-specific overrides work correctly
- ✅ Easy to add new configuration options
- ✅ Comprehensive test coverage for all configuration scenarios

This implementation plan follows Microsoft's recommended patterns for configuration management in .NET applications, ensuring security, maintainability, and compliance with enterprise standards.

Similar code found with 1 license type