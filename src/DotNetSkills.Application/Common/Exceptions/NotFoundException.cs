namespace DotNetSkills.Application.Common.Exceptions;

/// <summary>
/// Thrown when an entity or resource cannot be found.
/// </summary>
public sealed class NotFoundException : ApplicationExceptionBase
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }

    public override int StatusCode => 404;
    public override string ErrorCode => "not_found";
}
