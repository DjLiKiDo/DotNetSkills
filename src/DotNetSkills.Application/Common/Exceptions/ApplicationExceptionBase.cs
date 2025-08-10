namespace DotNetSkills.Application.Common.Exceptions;

/// <summary>
/// Base exception type for Application layer errors that should be translated to ProblemDetails.
/// </summary>
public abstract class ApplicationExceptionBase : Exception
{
    protected ApplicationExceptionBase(string message) : base(message) { }
    protected ApplicationExceptionBase(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Desired HTTP status code to surface for this exception type.
    /// </summary>
    public abstract int StatusCode { get; }

    /// <summary>
    /// A short, machine-friendly error code string (e.g., "not_found").
    /// </summary>
    public abstract string ErrorCode { get; }
}
