namespace ManagerGame.Core;

public class Result<T>
{
    private Result(bool isSuccess,
        Error error,
        T? value)
    {
        if (isSuccess && error != Error.None) throw new InvalidOperationException("Cannot be success and have error");
        if (!isSuccess && Value != null) throw new InvalidOperationException("Cannot be failure and have value");

        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; }
    public T? Value { get; set; }

    public static Result<T> Success(T value) => new(true, Error.None, value);

    public static Result<T> Failure(Error error) => new(false, error, default);
    public static Result<T> Failure(string error) => new(false, new Error(error), default);
}

public record Error(string Code)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NotFound = new("Not Found");
}
