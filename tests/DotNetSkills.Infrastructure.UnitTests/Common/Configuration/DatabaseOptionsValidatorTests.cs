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
        var options = new DatabaseOptions
        {
            ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=test;Trusted_Connection=True;",
            CommandTimeout = 30,
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5
        };

        var result = _validator.Validate(string.Empty, options);
        result.Should().Be(ValidateOptionsResult.Success);
    }

    [Fact]
    public void Validate_WithEmptyConnectionString_ShouldReturnFailure()
    {
        var options = new DatabaseOptions
        {
            ConnectionString = string.Empty,
            CommandTimeout = 30,
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5
        };

        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("DatabaseOptions.ConnectionString"));
    }
}
