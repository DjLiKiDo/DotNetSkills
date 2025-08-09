namespace DotNetSkills.Application.Common.Models;

/// <summary>
/// Represents the result of an operation without a return value.
/// Provides a way to handle success/failure states without throwing exceptions.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the Result class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="error">The error message if the operation failed.</param>
    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && !string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("A successful result cannot have an error message.", nameof(error));
        
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("A failed result must have an error message.", nameof(error));

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>A successful result with the specified value.</returns>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a failed result with a value type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);

    /// <summary>
    /// Implicitly converts a string error message to a failed Result.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation with a return value.
/// Provides a way to handle success/failure states with typed return values without throwing exceptions.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Initializes a new instance of the Result{T} class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="value">The value if the operation was successful.</param>
    /// <param name="error">The error message if the operation failed.</param>
    protected Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        if (isSuccess && value == null && !typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
            throw new ArgumentException("A successful result must have a non-null value for reference types.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A successful result with the specified value.</returns>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result.</returns>
    public new static Result<T> Failure(string error) => new(false, default, error);

    /// <summary>
    /// Implicitly converts a value to a successful Result{T}.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts a string error message to a failed Result{T}.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static implicit operator Result<T>(string error) => Failure(error);
}

/// <summary>
/// Extension methods for the Result pattern to support functional programming patterns.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes a function if the result is successful, otherwise returns the original failure.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="result">The input result.</param>
    /// <param name="func">The function to execute on success.</param>
    /// <returns>A new result with the transformed value or the original failure.</returns>
    public static Result<TOutput> Map<T, TOutput>(this Result<T> result, Func<T, TOutput> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        return result.IsSuccess && result.Value != null 
            ? Result<TOutput>.Success(func(result.Value))
            : Result<TOutput>.Failure(result.Error ?? "Unknown error");
    }

    /// <summary>
    /// Executes a function if the result is successful, otherwise returns the original failure.
    /// The function itself returns a Result, allowing for chaining of operations that might fail.
    /// </summary>
    /// <typeparam name="T">The input type.</typeparam>
    /// <typeparam name="TOutput">The output type.</typeparam>
    /// <param name="result">The input result.</param>
    /// <param name="func">The function to execute on success.</param>
    /// <returns>A new result from the function or the original failure.</returns>
    public static Result<TOutput> Bind<T, TOutput>(this Result<T> result, Func<T, Result<TOutput>> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        return result.IsSuccess && result.Value != null
            ? func(result.Value)
            : Result<TOutput>.Failure(result.Error ?? "Unknown error");
    }

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsSuccess && result.Value != null)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Executes an action if the result is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsFailure && result.Error != null)
            action(result.Error);

        return result;
    }
}