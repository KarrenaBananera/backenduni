namespace AngularApp3.Server.Common;

public class ServiceResult<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public int StatusCode { get; init; } = StatusCodes.Status200OK;

    public static ServiceResult<T> Success(T data, int statusCode = StatusCodes.Status200OK) =>
        new() { Succeeded = true, Data = data, StatusCode = statusCode };

    public static ServiceResult<T> Failure(string error, int statusCode) =>
        new() { Succeeded = false, Error = error, StatusCode = statusCode };
}
