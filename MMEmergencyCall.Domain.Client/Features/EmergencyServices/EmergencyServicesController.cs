namespace MMEmergencyCall.Domain.Client.Features.EmergencyServices;

[UserAuthorize]
[Route("api/[controller]")]
[ApiController]
public class EmergencyServicesController : BaseController
{
    private readonly EmergencyServiceService _emergencyServiceService;

    public EmergencyServicesController(EmergencyServiceService emergencyServiceService)
    {
        _emergencyServiceService = emergencyServiceService;
    }

    [HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
    public async Task<IActionResult> GetAllByPaginationAsync(int pageNo, int pageSize, string? serviceType)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _emergencyServiceService
            .GetEmergencyServices(pageNo, pageSize, serviceType);
        return Execute(model);
    }

    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetEmergencyServiceById(int serviceId)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }
        var response = await _emergencyServiceService.GetEmergencyServiceById(serviceId);
        return Execute(response);
    }

    [HttpGet("/api/EmergencyServices/Distance")]
    public async Task<IActionResult> GetEmergencyServiceWithinDistanceAsync(string? townshipCode, string? emergencyType, decimal lat, decimal lng, decimal maxDistanceInKm, int pageNo=1 , int pageSize=10)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }
        var response = await _emergencyServiceService.GetEmergencyServiceWithinDistanceAsync(townshipCode, emergencyType, lat, lng, maxDistanceInKm, pageNo, pageSize);
        return Execute(response);
    }

    [HttpPost]
    [UserAuthorize]
    public async Task<IActionResult> CreateEmergencyServiceAsync(
        EmergencyServiceRequestModel requestModel
    )
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }
        var model = await _emergencyServiceService.CreateEmergencyServiceAsync(requestModel, currentUserId.Value);
        return Execute(model);
    }

    [HttpPut("{id}")]
    [UserAuthorize]
    public async Task<IActionResult> UpdateEmergencyService(int id,
        [FromBody] EmergencyServiceRequestModel requestModel)
    {
        var currentUserId = HttpContext.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        Result<EmergencyServiceResponseModel> model = null;

        if (string.IsNullOrEmpty(requestModel.ServiceType))
        {
            model = Result<EmergencyServiceResponseModel>.ValidationError(
                "Service Type is required."
            );
            goto BadRequest;
        }

        if (string.IsNullOrEmpty(requestModel.ServiceGroup))
        {
            model = Result<EmergencyServiceResponseModel>.ValidationError(
                "Service Group is required."
            );
            goto BadRequest;
        }

        if (string.IsNullOrEmpty(requestModel.ServiceName))
        {
            model = Result<EmergencyServiceResponseModel>.ValidationError(
                "Service Name is required."
            );
            goto BadRequest;
        }

        if (string.IsNullOrEmpty(requestModel.PhoneNumber))
        {
            model = Result<EmergencyServiceResponseModel>.ValidationError(
                "Phone Number is required."
            );
            goto BadRequest;
        }

        model = await _emergencyServiceService.UpdateEmergencyService(id, currentUserId.Value, requestModel);

        return Execute(model);

        BadRequest:
        return Execute(model);
    }

    [HttpDelete("{id}")]
    [UserAuthorize]
    public async Task<IActionResult> DeleteEmergencyService(int id)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _emergencyServiceService.DeleteEmergencyService(id, currentUserId.Value);
        return Execute(model);
    }

}
