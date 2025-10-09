using Demoulas.ProfitSharing.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demoulas.ProfitSharing.Common.Extensions;

public static class ResultHttpExtensions
{
    /// <summary>
    /// Maps a domain Result<T> to a minimal API union result using standard patterns.
    /// NotFound mapping is triggered when the ProblemDetails.Detail matches a known not-found error description.
    /// </summary>
    public static Results<Ok<T>, NotFound, ProblemHttpResult> ToHttpResult<T>(this Result<T> result, params Error[] notFoundErrors)
    {
        return result.Match<Results<Ok<T>, NotFound, ProblemHttpResult>>(
            v => TypedResults.Ok(v),
            pd => notFoundErrors.Any(e => e.Description == pd.Detail)
                ? TypedResults.NotFound()
                : TypedResults.Problem(pd.Detail));
    }

    /// <summary>
    /// Maps a domain Result<T> to a minimal API union result with BadRequest support for validation errors.
    /// NotFound mapping is triggered when the ProblemDetails.Detail matches a known not-found error description.
    /// BadRequest mapping is triggered when the error has validation errors (Status 400).
    /// </summary>
    public static Results<Ok<T>, NotFound, BadRequest, ProblemHttpResult> ToHttpResultWithValidation<T>(this Result<T> result, params Error[] notFoundErrors)
    {
        return result.Match<Results<Ok<T>, NotFound, BadRequest, ProblemHttpResult>>(
            v => TypedResults.Ok(v),
            pd =>
            {
                if (notFoundErrors.Any(e => e.Description == pd.Detail))
                {
                    return TypedResults.NotFound();
                }
                if (pd.Status == 400 && result.Error?.ValidationErrors.Count > 0)
                {
                    return TypedResults.BadRequest();
                }
                return TypedResults.Problem(pd.Detail);
            });
    }

    /// <summary>
    /// Convenience method to convert a possibly-null value into a Result<T> using provided not-found error.
    /// </summary>
    public static Result<T> ToResultOrNotFound<T>(this T? value, Error notFoundError) where T : class
    {
        return value is null ? Result<T>.Failure(notFoundError) : Result<T>.Success(value);
    }
}
