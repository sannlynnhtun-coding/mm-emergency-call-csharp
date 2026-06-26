using Microsoft.AspNetCore.Mvc;
using MMEmergencyCall.Domain.Admin.Features;
using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests;

public class ResultPatternTests
{
    private readonly TestController _controller = new();

    [Fact]
    public void Result_factories_set_expected_flags()
    {
        Assert.True(Result<string>.Success("ok").IsSuccess);
        Assert.True(Result<string>.ValidationError("bad").IsValidationError());
        Assert.True(Result<string>.BadRequestError().IsBadRequest());
        Assert.True(Result<string>.InvalidDataError().IsInvalidData());
        Assert.True(Result<string>.DuplicateRecordError().IsDuplicateRecord());
        Assert.True(Result<string>.UnauthorizedError().IsUnauthorized());
        Assert.True(Result<string>.NotFoundError().IsNotFound());
        Assert.True(Result<string>.SystemError("boom").IsSystemError());
    }

    [Theory]
    [MemberData(nameof(StatusCases))]
    public void Base_controller_maps_result_to_http_status(Result<object?> result, int expectedStatus)
    {
        var actionResult = _controller.Execute(result);

        var actualStatus = actionResult switch
        {
            ObjectResult objectResult => objectResult.StatusCode ?? 200,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            _ => throw new InvalidOperationException(actionResult.GetType().Name)
        };

        Assert.Equal(expectedStatus, actualStatus);
    }

    public static IEnumerable<object[]> StatusCases()
    {
        yield return new object[] { Result<object?>.Success(new { }), 200 };
        yield return new object[] { Result<object?>.BadRequestError(), 400 };
        yield return new object[] { Result<object?>.ValidationError("bad"), 400 };
        yield return new object[] { Result<object?>.InvalidDataError(), 400 };
        yield return new object[] { Result<object?>.UnauthorizedError(), 401 };
        yield return new object[] { Result<object?>.NotFoundError(), 404 };
        yield return new object[] { Result<object?>.DuplicateRecordError(), 409 };
        yield return new object[] { Result<object?>.SystemError("Internal server error"), 500 };
    }

    private sealed class TestController : BaseController
    {
    }
}
