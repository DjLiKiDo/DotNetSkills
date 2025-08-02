namespace DotNetSkills.Domain.UnitTests.Builders;

/// <summary>
/// Builder for creating test UserId instances.
/// </summary>
public class UserIdBuilder
{
    private Guid _value = Guid.NewGuid();

    public static UserIdBuilder Create() => new();

    public UserIdBuilder WithValue(Guid value)
    {
        _value = value;
        return this;
    }

    public UserIdBuilder WithNewValue()
    {
        _value = Guid.NewGuid();
        return this;
    }

    public UserIdBuilder WithEmptyValue()
    {
        _value = Guid.Empty;
        return this;
    }

    public UserId Build() => new(_value);

    public static implicit operator UserId(UserIdBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test TeamId instances.
/// </summary>
public class TeamIdBuilder
{
    private Guid _value = Guid.NewGuid();

    public static TeamIdBuilder Create() => new();

    public TeamIdBuilder WithValue(Guid value)
    {
        _value = value;
        return this;
    }

    public TeamIdBuilder WithNewValue()
    {
        _value = Guid.NewGuid();
        return this;
    }

    public TeamIdBuilder WithEmptyValue()
    {
        _value = Guid.Empty;
        return this;
    }

    public TeamId Build() => new(_value);

    public static implicit operator TeamId(TeamIdBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test ProjectId instances.
/// </summary>
public class ProjectIdBuilder
{
    private Guid _value = Guid.NewGuid();

    public static ProjectIdBuilder Create() => new();

    public ProjectIdBuilder WithValue(Guid value)
    {
        _value = value;
        return this;
    }

    public ProjectIdBuilder WithNewValue()
    {
        _value = Guid.NewGuid();
        return this;
    }

    public ProjectIdBuilder WithEmptyValue()
    {
        _value = Guid.Empty;
        return this;
    }

    public ProjectId Build() => new(_value);

    public static implicit operator ProjectId(ProjectIdBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test TaskId instances.
/// </summary>
public class TaskIdBuilder
{
    private Guid _value = Guid.NewGuid();

    public static TaskIdBuilder Create() => new();

    public TaskIdBuilder WithValue(Guid value)
    {
        _value = value;
        return this;
    }

    public TaskIdBuilder WithNewValue()
    {
        _value = Guid.NewGuid();
        return this;
    }

    public TaskIdBuilder WithEmptyValue()
    {
        _value = Guid.Empty;
        return this;
    }

    public TaskId Build() => new(_value);

    public static implicit operator TaskId(TaskIdBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test TeamMemberId instances.
/// </summary>
public class TeamMemberIdBuilder
{
    private Guid _value = Guid.NewGuid();

    public static TeamMemberIdBuilder Create() => new();

    public TeamMemberIdBuilder WithValue(Guid value)
    {
        _value = value;
        return this;
    }

    public TeamMemberIdBuilder WithNewValue()
    {
        _value = Guid.NewGuid();
        return this;
    }

    public TeamMemberIdBuilder WithEmptyValue()
    {
        _value = Guid.Empty;
        return this;
    }

    public TeamMemberId Build() => new(_value);

    public static implicit operator TeamMemberId(TeamMemberIdBuilder builder) => builder.Build();
}

/// <summary>
/// Builder for creating test EmailAddress instances.
/// </summary>
public class EmailAddressBuilder
{
    private string _value = "test@example.com";

    public static EmailAddressBuilder Create() => new();

    public EmailAddressBuilder WithValue(string value)
    {
        _value = value;
        return this;
    }

    public EmailAddressBuilder WithValidEmail(string localPart = "test", string domain = "example.com")
    {
        _value = $"{localPart}@{domain}";
        return this;
    }

    public EmailAddressBuilder WithInvalidFormat()
    {
        _value = "invalid-email";
        return this;
    }

    public EmailAddressBuilder WithMissingAtSymbol()
    {
        _value = "testexample.com";
        return this;
    }

    public EmailAddressBuilder WithMissingDomain()
    {
        _value = "test@";
        return this;
    }

    public EmailAddressBuilder WithEmptyLocalPart()
    {
        _value = "@example.com";
        return this;
    }

    public EmailAddressBuilder WithLongEmail()
    {
        var longLocalPart = new string('a', 200);
        _value = $"{longLocalPart}@example.com";
        return this;
    }

    public EmailAddressBuilder WithWhitespace()
    {
        _value = "  test@example.com  ";
        return this;
    }

    public EmailAddressBuilder WithUpperCase()
    {
        _value = "TEST@EXAMPLE.COM";
        return this;
    }

    public EmailAddress Build() => new(_value);

    public string BuildInvalidValue() => _value;

    public static implicit operator EmailAddress(EmailAddressBuilder builder) => builder.Build();
}