using DotNetSkills.Infrastructure.Common.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.UnitTests.Options;

public class DatabaseOptionsValidatorTests
{
    private readonly IValidateOptions<DatabaseOptions> _validator = new DatabaseOptionsValidator();

    [Fact]
    public void Validate_NullConnectionString_Fails()
    {
        var options = new DatabaseOptions { ConnectionString = "", CommandTimeout = 30, MaxRetryCount = 3, MaxRetryDelaySeconds = 5 };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("ConnectionString is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(301)]
    public void Validate_InvalidCommandTimeout_Fails(int timeout)
    {
        var options = new DatabaseOptions { ConnectionString = "Server=.;Database=x;Trusted_Connection=True;", CommandTimeout = timeout, MaxRetryCount = 3, MaxRetryDelaySeconds = 5 };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("CommandTimeout");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public void Validate_InvalidRetryCount_Fails(int count)
    {
        var options = new DatabaseOptions { ConnectionString = "Server=.;Database=x;Trusted_Connection=True;", CommandTimeout = 30, MaxRetryCount = count, MaxRetryDelaySeconds = 5 };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MaxRetryCount");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    public void Validate_InvalidRetryDelay_Fails(int seconds)
    {
        var options = new DatabaseOptions { ConnectionString = "Server=.;Database=x;Trusted_Connection=True;", CommandTimeout = 30, MaxRetryCount = 3, MaxRetryDelaySeconds = seconds };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("MaxRetryDelaySeconds");
    }

    [Fact]
    public void Validate_Valid_Succeeds()
    {
        var options = new DatabaseOptions
        {
            ConnectionString = "Server=.;Database=x;Trusted_Connection=True;",
            CommandTimeout = 30,
            MaxRetryCount = 3,
            MaxRetryDelaySeconds = 5,
            EnableDetailedErrors = false,
            EnableSensitiveDataLogging = false,
            EnableQueryLogging = false
        };

        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }
}
