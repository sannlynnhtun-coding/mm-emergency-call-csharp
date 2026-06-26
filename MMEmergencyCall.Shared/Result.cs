namespace MMEmergencyCall.Shared;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsError => !IsSuccess;
    public T? Data { get; private set; }
    public string Message { get; private set; } = string.Empty;

    private EnumResultType Type { get; set; }

    public bool IsValidationError() => Type == EnumResultType.ValidationError;
    public bool IsSystemError() => Type == EnumResultType.SystemError;
    public bool IsDataError() => Type == EnumResultType.Error;
    public bool IsNotFound() => Type == EnumResultType.NotFound;
    public bool IsDuplicateRecord() => Type == EnumResultType.DuplicateRecord;
    public bool IsInvalidData() => Type == EnumResultType.InvalidData;
    public bool IsBadRequest() => Type == EnumResultType.BadRequest;
    public bool IsUnauthorized() => Type == EnumResultType.Unauthorized;

    public static Result<T> Success(T data, string message = "Success") =>
        new() { IsSuccess = true, Type = EnumResultType.Success, Data = data, Message = message };

    public static Result<T> ValidationError(string message, T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.ValidationError, Data = data, Message = message };

    public static Result<T> SystemError(string message, T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.SystemError, Data = data, Message = message };

    public static Result<T> Error(string message = "Some Error Occurred", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.Error, Data = data, Message = message };

    public static Result<T> DuplicateRecordError(string message = "Duplicate Record", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.DuplicateRecord, Data = data, Message = message };

    public static Result<T> NotFoundError(string message = "Not Found", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.NotFound, Data = data, Message = message };

    public static Result<T> InvalidDataError(string message = "Invalid Data", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.InvalidData, Data = data, Message = message };

    public static Result<T> BadRequestError(string message = "Bad Request", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.BadRequest, Data = data, Message = message };

    public static Result<T> UnauthorizedError(string message = "Unauthorized", T? data = default) =>
        new() { IsSuccess = false, Type = EnumResultType.Unauthorized, Data = data, Message = message };
}
