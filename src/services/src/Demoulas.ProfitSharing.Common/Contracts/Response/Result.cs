namespace Demoulas.ProfitSharing.Common.Contracts.Response;

public sealed record Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public T? Value { get; }

    public Result(T? value, bool isSuccess, string? error)
    {
        switch (isSuccess)
        {
            case true when error != null:
                throw new InvalidOperationException("A successful result cannot have an error message.");
            case false when string.IsNullOrEmpty(error):
                throw new InvalidOperationException("A failed result must have an error message.");
        }

        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value), "Success result value cannot be null.");
        }

        return new Result<T>(value, true, null);
    }

    public static Result<T> Failure(string error)
    {
        if (string.IsNullOrEmpty(error))
        {
            throw new ArgumentNullException(nameof(error), "Error message cannot be null or empty.");
        }

        return new Result<T>(default, false, error);
    }
}


