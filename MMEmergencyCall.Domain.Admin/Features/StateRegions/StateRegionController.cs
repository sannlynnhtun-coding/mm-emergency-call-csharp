namespace MMEmergencyCall.Domain.Admin.Features.StateRegions;

[Route("api/[controller]")]
[AdminAuthorize]
[ApiController]
public class StateRegionController : BaseController
{
    private readonly StateRegionService _stateRegionService;

    public StateRegionController(StateRegionService stateRegionService)
    {
        _stateRegionService = stateRegionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var model = await _stateRegionService.GetAllAsync();
        return Execute(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _stateRegionService.GetByIdAsync(id);
        return Execute(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StateRegionRequestModel requestModel)
    {
        Result<StateRegionResponseModel> model = null;
        if (string.IsNullOrEmpty(requestModel.StateRegionCode))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionCode is required.");
            goto badRequest;
        }
        if (string.IsNullOrEmpty(requestModel.StateRegionNameEn))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionNameEn is required.");
            goto badRequest;
        }
        if (string.IsNullOrEmpty(requestModel.StateRegionNameMm))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionNameMm is required.");
            goto badRequest;
        }

        model = await _stateRegionService.CreateAsync(requestModel);
        if (model.IsSuccess)
            return Execute(model);

        badRequest:
        return Execute(model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StateRegionRequestModel requestModel)
    {
        Result<StateRegionResponseModel> model = null;
        if (string.IsNullOrEmpty(requestModel.StateRegionCode))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionCode is required.");
            goto badRequest;
        }
        if (string.IsNullOrEmpty(requestModel.StateRegionNameEn))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionNameEn is required.");
            goto badRequest;
        }
        if (string.IsNullOrEmpty(requestModel.StateRegionNameMm))
        {
            model = Result<StateRegionResponseModel>.ValidationError("StateRegionNameMm is required.");
            goto badRequest;
        }

        model = await _stateRegionService.UpdateAsync(id, requestModel);
        if (!model.IsSuccess)
            return Execute(model);

        return Execute(model);

    badRequest:
        return Execute(model);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _stateRegionService.DeleteAsync(id);
        return Execute(model);
    }
}
