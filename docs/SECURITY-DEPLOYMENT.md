# Security Configuration for Production Deployment

This document outlines the security configuration required for production deployment of the DotNetSkills application.

## ⚠️ CRITICAL: Never Commit Secrets to Source Control

All production secrets must be configured through environment variables or Azure Key Vault. Hardcoded credentials have been removed from configuration files.

## Required Environment Variables

### For Non-Azure Deployments

Set these environment variables in your production environment:

```bash
# Database Configuration
DOTNETSKILLS_Database__ConnectionString="Server=your-server;Database=DotNetSkills;User Id=your-user;Password=your-password;TrustServerCertificate=false;Encrypt=true"

# JWT Authentication
DOTNETSKILLS_Jwt__SigningKey="YourSecureJWTSigningKeyThatIsAtLeast256BitsLongForHS256Algorithm"

# Azure Key Vault (if using Key Vault)
DOTNETSKILLS_KeyVault__Uri="https://your-keyvault-name.vault.azure.net/"
```

### For Azure App Service

Configure these application settings in your Azure App Service:

- `Database__ConnectionString` 
- `Jwt__SigningKey`
- `KeyVault__Uri` (if using Key Vault)

## Azure Key Vault Configuration (Recommended)

### 1. Create Secrets in Azure Key Vault

Create the following secrets in your Azure Key Vault:

```
Secret Name: DotNetSkills-Database--ConnectionString
Secret Value: Server=your-server;Database=DotNetSkills;User Id=your-user;Password=your-password;TrustServerCertificate=false;Encrypt=true

Secret Name: DotNetSkills-Jwt--SigningKey  
Secret Value: YourSecureJWTSigningKeyThatIsAtLeast256BitsLongForHS256Algorithm
```

### 2. Configure Managed Identity

For Azure App Service, enable system-assigned managed identity and grant it the following Key Vault permissions:

- **Secret permissions**: Get, List

### 3. Update Configuration

Set the Key Vault URI in your app configuration:

```json
{
  "KeyVault": {
    "Uri": "https://your-keyvault-name.vault.azure.net/"
  }
}
```

## JWT Signing Key Generation

Generate a secure JWT signing key using one of these methods:

### Method 1: PowerShell
```powershell
[System.Web.Security.Membership]::GeneratePassword(64, 0)
```

### Method 2: OpenSSL
```bash
openssl rand -base64 64
```

### Method 3: Online Generator
Use a reputable online key generator to create a 256-bit (or larger) key.

## Security Best Practices

1. **Use HTTPS Only**: Ensure TrustServerCertificate=false and Encrypt=true for database connections
2. **Rotate Secrets Regularly**: JWT signing keys and database passwords should be rotated quarterly
3. **Use Managed Identity**: When deploying to Azure, use managed identity instead of connection strings when possible
4. **Monitor Access**: Enable logging and monitoring for Key Vault access and authentication events
5. **Principle of Least Privilege**: Grant only the minimum required permissions

## Verification Steps

After deployment, verify security configuration:

1. **Check JWT is enabled**: GET `/health` should require authentication
2. **Verify secrets are loaded**: Application should start without errors
3. **Test authentication**: JWT tokens should be properly validated
4. **Review logs**: No sensitive data should appear in application logs

## Development vs Production

| Setting | Development | Production |
|---------|------------|------------|
| JWT Enabled | ✅ True | ✅ True |
| JWT Signing Key | Temporary key in appsettings.json | Environment variable or Key Vault |
| Database Connection | LocalDB | Azure SQL/Production database |
| HTTPS Required | Optional | ✅ Required |
| Detailed Errors | ✅ Enabled | ❌ Disabled |
| Query Logging | ✅ Enabled | ❌ Disabled |

## Troubleshooting

### Common Issues

1. **"JWT SigningKey is required"**: Ensure the JWT signing key is properly set via environment variables or Key Vault
2. **"Database connection string is invalid"**: Verify the connection string format and credentials
3. **"Key Vault access denied"**: Check managed identity permissions and Key Vault access policies
4. **"Configuration validation failed"**: Review all required configuration values are present

### Debugging Steps

1. Check application logs for configuration validation errors
2. Verify environment variables are set correctly
3. Test Key Vault connectivity using Azure CLI
4. Validate JWT signing key length (minimum 256 bits for HS256)

## Security Incident Response

If credentials are compromised:

1. **Immediate**: Rotate JWT signing keys
2. **Within 1 hour**: Change database passwords  
3. **Within 4 hours**: Review access logs for suspicious activity
4. **Within 24 hours**: Complete security assessment and update documentation

---

For additional security guidance, see the [OWASP Web Application Security Testing Guide](https://owasp.org/www-project-web-security-testing-guide/).