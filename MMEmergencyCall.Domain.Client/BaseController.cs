namespace MMEmergencyCall.Domain.Client;

public class BaseController : ControllerBase
{
    private IActionResult InternalServerError(object? obj = null)
    {
        return StatusCode(500, obj);
    }

    protected IActionResult Execute<T>(Result<T> model)
    {
        if (model is null)
            return InternalServerError(Result<T>.SystemError("Internal server error"));

        if (model.IsValidationError() || model.IsBadRequest() || model.IsInvalidData() || model.IsDataError())
            return BadRequest(model);

        if (model.IsUnauthorized())
            return Unauthorized(model);

        if (model.IsNotFound())
            return NotFound(model);

        if (model.IsDuplicateRecord())
            return Conflict(model);

        if (model.IsSystemError())
            return InternalServerError(model);

        return Ok(model);
    }

    protected IActionResult BadRequestResult(string message)
    {
        return Execute(Result<object?>.BadRequestError(message));
    }

    protected IActionResult UnauthorizedResult(string message = "Unauthorized Request")
    {
        return Execute(Result<object?>.UnauthorizedError(message));
    }
}
