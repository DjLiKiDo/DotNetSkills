namespace DotNetSkills.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation before handler execution.
/// Automatically validates any request that has registered validators and returns validation errors
/// in a consistent format using the Result pattern.
/// </summary>
/// <typeparam name="TRequest">The type of the request being validated.</typeparam>
/// <typeparam name="TResponse">The type of the response from the handler.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the ValidationBehavior class.
    /// </summary>
    /// <param name="validators">Collection of validators for the request type.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validates the request before passing it to the next handler in the pipeline.
    /// If validation fails, returns a failed Result with validation errors.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler or a validation failure result.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next().ConfigureAwait(false);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(v => 
            v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            return CreateValidationResult<TResponse>(failures);
        }

        return await next().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a validation result for the response type.
    /// Handles both Result and Result{T} response types.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="failures">The validation failures.</param>
    /// <returns>A failed result containing validation errors.</returns>
    private static T CreateValidationResult<T>(IEnumerable<ValidationFailure> failures) where T : class
    {
        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
        
        // Handle Result<TValue> types
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = typeof(T).GetGenericArguments()[0];
            var failureMethod = typeof(Result<>).MakeGenericType(valueType).GetMethod("Failure", new[] { typeof(string) });
            return (T)failureMethod!.Invoke(null, new object[] { errorMessage })!;
        }

        // Handle Result type
        if (typeof(T) == typeof(Result))
        {
            return (T)(object)Result.Failure(errorMessage);
        }

        // For non-Result types, throw validation exception as fallback
        throw new FluentValidation.ValidationException(failures);
    }
}