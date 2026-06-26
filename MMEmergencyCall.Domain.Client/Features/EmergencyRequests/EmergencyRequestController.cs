namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequests;

[Route("api/[controller]")]
[UserAuthorize]
[ApiController]
public class EmergencyRequestController : BaseController
{
    private readonly EmergencyRequestService _emergencyRequestService;

    public EmergencyRequestController(EmergencyRequestService emergencyRequestService)
    {
        _emergencyRequestService = emergencyRequestService;
    }

    [HttpGet("pageNo/{pageNo}/pageSize/{pageSize}")]
    public async Task<IActionResult> GetEmergencyRequests(string? serviceId, string? providerId,
        string? status, string? townshipCode, int pageNo = 1, int pageSize = 10)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue) { 
            return UnauthorizedResult();
        }

        if (pageNo <= 0 || pageSize <= 0)
        {
            return BadRequestResult("Page number and page size must be greater than zero.");
        }

        var model = await _emergencyRequestService.GetEmergencyRequests(pageNo, pageSize, currentUserId, serviceId,providerId,status,townshipCode);
        return Execute(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmergencyRequestById(int id)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _emergencyRequestService.GetEmergencyRequestById(id,currentUserId.Value);
        return Execute(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddEmergencyRequest(EmergencyRequestRequestModel request)
    {
        var currentUserId = HttpContext.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            return UnauthorizedResult();
        }

        var model = await _emergencyRequestService.AddEmergencyRequest(request,currentUserId.Value);
        return Execute(model);
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateEmergencyRequestStatus(int id, UpdateEmergencyRequestStatusRequest statusRequest)
    //{
    //    var currentUserId = HttpContext.GetCurrentUserId();

    //    if (!currentUserId.HasValue)
    //    {
    //        return UnauthorizedResult();
    //    }

    //    var model = await _emergencyRequestService.UpdateEmergencyRequestStatus(id, currentUserId.Value,statusRequest);
    //    return Execute(model);
    //}
}
