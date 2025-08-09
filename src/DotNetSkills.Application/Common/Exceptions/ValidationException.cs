using FluentValidation.Results;

namespace DotNetSkills.Application.Common.Exceptions;

/// <summary>
/// Wrapper for FluentValidation failures to unify ProblemDetails mapping.
/// </summary>
public sealed class ValidationException : ApplicationExceptionBase
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Validation failed for the request.")
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).Distinct().ToArray());
    }

    public override int StatusCode => 400;
    public override string ErrorCode => "validation_failed";
}
