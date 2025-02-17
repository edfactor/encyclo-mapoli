using Microsoft.AspNetCore.Mvc;

namespace Demoulas.ProfitSharing.Common.Contracts
{
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
    }
}
