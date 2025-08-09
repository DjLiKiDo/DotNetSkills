namespace DotNetSkills.Infrastructure.Persistence.Extensions;

/// <summary>
/// Extension methods for EntityTypeBuilder to provide common entity configuration patterns.
/// Simplifies repetitive configuration tasks and ensures consistency across entity configurations.
/// </summary>
public static class EntityConfigurationExtensions
{
    /// <summary>
    /// Configures a required string property with maximum length and indexing.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <param name="maxLength">Maximum length for the string property.</param>
    /// <param name="columnType">Optional SQL column type (defaults to nvarchar).</param>
    /// <param name="createIndex">Whether to create an index on this property.</param>
    /// <param name="isUnique">Whether the index should be unique.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<string> ConfigureRequiredString<TEntity>(
        this PropertyBuilder<string> builder,
        int maxLength,
        string? columnType = null,
        bool createIndex = false,
        bool isUnique = false)
        where TEntity : class
    {
        builder
            .IsRequired()
            .HasMaxLength(maxLength);
            
        if (!string.IsNullOrEmpty(columnType))
        {
            builder.HasColumnType(columnType);
        }
        
        return builder;
    }
    
    /// <summary>
    /// Configures an optional string property with maximum length.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <param name="maxLength">Maximum length for the string property.</param>
    /// <param name="columnType">Optional SQL column type (defaults to nvarchar).</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<string?> ConfigureOptionalString<TEntity>(
        this PropertyBuilder<string?> builder,
        int maxLength,
        string? columnType = null)
        where TEntity : class
    {
        builder
            .IsRequired(false)
            .HasMaxLength(maxLength);
            
        if (!string.IsNullOrEmpty(columnType))
        {
            builder.HasColumnType(columnType);
        }
        
        return builder;
    }
    
    /// <summary>
    /// Configures a DateTime property with proper SQL Server datetime2 precision.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="precision">The precision for datetime2 (default is 7).</param>
    /// <param name="defaultValueSql">Optional default value SQL expression.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<DateTime> ConfigureDateTime(
        this PropertyBuilder<DateTime> builder,
        int precision = 7,
        string? defaultValueSql = null)
    {
        builder
            .IsRequired()
            .HasColumnType($"datetime2({precision})");
            
        if (!string.IsNullOrEmpty(defaultValueSql))
        {
            builder.HasDefaultValueSql(defaultValueSql);
        }
        
        return builder;
    }
    
    /// <summary>
    /// Configures a nullable DateTime property with proper SQL Server datetime2 precision.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="precision">The precision for datetime2 (default is 7).</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<DateTime?> ConfigureOptionalDateTime(
        this PropertyBuilder<DateTime?> builder,
        int precision = 7)
    {
        builder
            .IsRequired(false)
            .HasColumnType($"datetime2({precision})");
        
        return builder;
    }
    
    /// <summary>
    /// Configures a decimal property with specific precision and scale.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="precision">Total number of digits.</param>
    /// <param name="scale">Number of digits after decimal point.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<decimal> ConfigureDecimal(
        this PropertyBuilder<decimal> builder,
        int precision = 18,
        int scale = 2)
    {
        builder
            .IsRequired()
            .HasPrecision(precision, scale);
        
        return builder;
    }
    
    /// <summary>
    /// Configures a nullable decimal property with specific precision and scale.
    /// </summary>
    /// <param name="builder">The property builder.</param>
    /// <param name="precision">Total number of digits.</param>
    /// <param name="scale">Number of digits after decimal point.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<decimal?> ConfigureOptionalDecimal(
        this PropertyBuilder<decimal?> builder,
        int precision = 18,
        int scale = 2)
    {
        builder
            .IsRequired(false)
            .HasPrecision(precision, scale);
        
        return builder;
    }
    
    /// <summary>
    /// Configures an enum property to be stored as a string in the database.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <param name="maxLength">Maximum length for the enum string value.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<TEnum> ConfigureEnumAsString<TEnum>(
        this PropertyBuilder<TEnum> builder,
        int maxLength = 50)
        where TEnum : struct, Enum
    {
        builder
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(maxLength);
        
        return builder;
    }
    
    /// <summary>
    /// Configures a nullable enum property to be stored as a string in the database.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="builder">The property builder.</param>
    /// <param name="maxLength">Maximum length for the enum string value.</param>
    /// <returns>The property builder for method chaining.</returns>
    public static PropertyBuilder<TEnum?> ConfigureOptionalEnumAsString<TEnum>(
        this PropertyBuilder<TEnum?> builder,
        int maxLength = 50)
        where TEnum : struct, Enum
    {
        builder
            .IsRequired(false)
            .HasConversion<string>()
            .HasMaxLength(maxLength);
        
        return builder;
    }
    
    /// <summary>
    /// Configures a foreign key property with proper naming and indexing.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TRelatedEntity">The related entity type.</typeparam>
    /// <param name="entityBuilder">The entity type builder.</param>
    /// <param name="foreignKeyExpression">Expression for the foreign key property.</param>
    /// <param name="principalKeyExpression">Expression for the principal key property.</param>
    /// <param name="onDeleteBehavior">The delete behavior for the relationship.</param>
    /// <returns>The reference collection builder for method chaining.</returns>
    public static ReferenceCollectionBuilder<TRelatedEntity, TEntity> ConfigureForeignKey<TEntity, TRelatedEntity>(
        this EntityTypeBuilder<TEntity> entityBuilder,
        Expression<Func<TEntity, object?>> foreignKeyExpression,
        Expression<Func<TRelatedEntity, object?>> principalKeyExpression,
        DeleteBehavior onDeleteBehavior = DeleteBehavior.Restrict)
        where TEntity : class
        where TRelatedEntity : class
    {
        return entityBuilder
            .HasOne<TRelatedEntity>()
            .WithMany()
            .HasForeignKey(foreignKeyExpression)
            .HasPrincipalKey(principalKeyExpression)
            .OnDelete(onDeleteBehavior);
    }
    
    /// <summary>
    /// Creates a composite index on multiple properties.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="builder">The entity type builder.</param>
    /// <param name="propertyNames">Names of the properties to include in the index.</param>
    /// <param name="isUnique">Whether the index should be unique.</param>
    /// <param name="indexName">Optional custom index name.</param>
    /// <returns>The index builder for method chaining.</returns>
    public static IndexBuilder CreateCompositeIndex<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string[] propertyNames,
        bool isUnique = false,
        string? indexName = null)
        where TEntity : class
    {
        if (propertyNames == null || propertyNames.Length == 0)
            throw new ArgumentException("At least one property name is required.", nameof(propertyNames));
            
        var indexBuilder = builder.HasIndex(propertyNames);
        
        if (isUnique)
        {
            indexBuilder.IsUnique();
        }
        
        if (!string.IsNullOrEmpty(indexName))
        {
            indexBuilder.HasDatabaseName(indexName);
        }
        else
        {
            var entityName = typeof(TEntity).Name;
            var compositeIndexName = $"IX_{entityName}_{string.Join("_", propertyNames)}";
            indexBuilder.HasDatabaseName(compositeIndexName);
        }
        
        return indexBuilder;
    }
}
