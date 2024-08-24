namespace ManagerGame.Commands;

public class Result
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Cannot be success and have error");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<T> where T : class
{
	private Result(bool isSuccess, Error error, T? value)
	{
		if (isSuccess && error != Error.None)
		{
			throw new InvalidOperationException("Cannot be success and have error");
		}

		IsSuccess = isSuccess;
		Error = error;
		Value = value;
	}

	public T? Value { get; private set; }

	public bool IsSuccess { get; set; }
	public bool IsFailure => !IsSuccess;
	public Error Error { get; set; }

	public static Result<T> Success(T value) => new(true, Error.None, value);
	public static Result<T> Failure(Error error) => new(false, error, null);
}

public record Error(string Code)
{
    public static readonly Error None = new(string.Empty);
    public static readonly Error NotFound = new("Not Found");
}
