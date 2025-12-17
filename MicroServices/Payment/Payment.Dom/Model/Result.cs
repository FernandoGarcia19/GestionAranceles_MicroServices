namespace Payment.Dom.Model;

public class Result
{
    public bool IsSuccess { get; }
    public List<string> Errors { get; }

    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, List<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new Result(true, new List<string>());
    public static Result Failure(params string[] errors) => new Result(false, new List<string>(errors));
}

public class Result<T> : Result
{
    public T Value { get; }

    private Result(bool isSuccess, T value, List<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }
    public static Result<T> Success(T value) => new Result<T>(true, value, new List<string>());
    public new static Result<T> Failure(params string[] errors) => new Result<T>(false, default(T)!, new List<string>(errors));
}
