namespace MMEmergencyCall.Domain.Client.Features.EmergencyServiceType;

[Route("api/EmergencyService/ServiceType")]
[ApiController]
public class EmergencyServiceTypeController : BaseController
{
    private readonly EmergencyServiceTypeService _emergencyServiceTypeService;

    public EmergencyServiceTypeController(
        EmergencyServiceTypeService emergencyServiceTypeService
    )
    {
        _emergencyServiceTypeService = emergencyServiceTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmergencyServiceTypesAsync()
    {
        var response = await _emergencyServiceTypeService.GetServiceTypesAsync();
        return Execute(response);
    }
}