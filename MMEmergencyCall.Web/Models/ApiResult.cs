namespace MMEmergencyCall.Web.Models;

public sealed class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public bool IsError { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ApiResult<T> Failure(string message) =>
        new() { IsSuccess = false, IsError = true, Message = message };
}
