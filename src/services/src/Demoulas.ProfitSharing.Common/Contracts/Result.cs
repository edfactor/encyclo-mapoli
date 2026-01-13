using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Common.Contracts;

/// <summary>
/// Represents the result of an operation, encapsulating either a successful value or an error.
/// </summary>
/// <typeparam name="T">The type of the value returned in case of success.</typeparam>
public sealed record Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess { get; }
    public bool IsError => !IsSuccess;

    private Result(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null.");
        IsSuccess = true;
        Error = null;
    }

    private Result(Error error)
    {
        Error = error ?? throw new ArgumentNullException(nameof(error), "Error cannot be null.");
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

    public static Result<T> ValidationFailure(Dictionary<string, string[]> validationErrors) =>
        new(Error.Validation(validationErrors));

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ProblemDetails, TResult> onError)
    {
        return IsSuccess ? onSuccess(Value!) : onError(Error!);
    }

    /// <summary>
    /// Converts this result into an HTTP union using supplied not-found errors. Use explicitly when not using implicit cast.
    /// </summary>
    public Results<Ok<T>, NotFound, ProblemHttpResult> ToHttpResult(params Error[] notFoundErrors)
    {
        if (IsSuccess)
        {
            return TypedResults.Ok(Value!);
        }

        ProblemDetails pd = Error!;
        if (notFoundErrors.Any(e => e.Description == pd.Detail))
        {
            return TypedResults.NotFound();
        }
        return TypedResults.Problem(pd.Detail);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Result{T}"/> to <see cref="Results{Ok, NotFound, ProblemHttpResult}"/> assuming no not-found semantics.
    /// Preferred only when caller already encoded appropriate error description for not-found.
    /// For specific not-found mapping, call <see cref="ToHttpResult"/> explicitly.
    /// </summary>
    public static implicit operator Results<Ok<T>, NotFound, ProblemHttpResult>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value!);
        }

        ProblemDetails pd = result.Error!; // implicit conversion from Error
        return TypedResults.Problem(pd.Detail);
    }
}
