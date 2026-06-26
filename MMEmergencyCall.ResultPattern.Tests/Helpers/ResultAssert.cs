using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests.Helpers;

public static class ResultAssert
{
    public static T Success<T>(Result<T> result)
    {
        Assert.True(result.IsSuccess, result.Message);
        Assert.NotNull(result.Data);
        return result.Data!;
    }

    public static void Validation<T>(Result<T> result)
    {
        Assert.True(result.IsValidationError(), result.Message);
    }

    public static void BadRequest<T>(Result<T> result)
    {
        Assert.True(result.IsBadRequest(), result.Message);
    }

    public static void NotFound<T>(Result<T> result)
    {
        Assert.True(result.IsNotFound(), result.Message);
    }

    public static void Duplicate<T>(Result<T> result)
    {
        Assert.True(result.IsDuplicateRecord(), result.Message);
    }

    public static void InvalidData<T>(Result<T> result)
    {
        Assert.True(result.IsInvalidData(), result.Message);
    }

    public static void SystemError<T>(Result<T> result)
    {
        Assert.True(result.IsSystemError(), result.Message);
    }
}
