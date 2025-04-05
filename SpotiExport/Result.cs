namespace SpotiExport;

internal sealed class Result<T>
{
    public bool Success { get; set; }
    public bool Failed => !Success;
    public T? ResultValue { get; set; }
    public string? ErrorMessage { get; set; }

    public Result(bool success, T? resultValue = default, string? errorMessage = null)
    {
        Success = success;
        ResultValue = resultValue;
        ErrorMessage = errorMessage;
    }
    
    public static Result<T> Ok(T data) => new Result<T>(true, data);
    public static Result<T> Fail(string errorMessage) => new Result<T>(false, default, errorMessage);
}