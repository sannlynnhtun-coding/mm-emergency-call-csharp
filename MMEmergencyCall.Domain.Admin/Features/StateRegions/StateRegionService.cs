namespace MMEmergencyCall.Domain.Admin.Features.StateRegions;

public class StateRegionService
{
    private readonly ILogger<StateRegionService> _logger;
    private readonly AppDbContext _context;

    public StateRegionService(ILogger<StateRegionService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<StateRegionResponseModel>> CreateAsync(StateRegionRequestModel requestModel)
    {
        try
        {
            var stateRegion = new StateRegion
            {
                StateRegionCode = requestModel.StateRegionCode,
                StateRegionNameEn = requestModel.StateRegionNameEn,
                StateRegionNameMm = requestModel.StateRegionNameMm
            };

            _context.Add(stateRegion);
            await _context.SaveChangesAsync();

            var model = new StateRegionResponseModel
            {
                StateRegionId = stateRegion.StateRegionId,
                StateRegionCode = stateRegion.StateRegionCode,
                StateRegionNameEn = stateRegion.StateRegionNameEn,
                StateRegionNameMm = stateRegion.StateRegionNameMm
            };

            return Result<StateRegionResponseModel>.Success(model, "State region created successfully.");
        }
        catch (Exception ex)
        {
            string message = "An error occurred while creating the state region requests: " + ex.Message;
            _logger.LogError(message);
            return Result<StateRegionResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<StateRegionResponseModel>> GetByIdAsync(int id)
    {
        try
        {
            var stateRegion = await _context.Set<StateRegion>().FindAsync(id);
            if (stateRegion == null)
            {
                return Result<StateRegionResponseModel>.NotFoundError("State region not found.");
            }

            var model = new StateRegionResponseModel
            {
                StateRegionId = stateRegion.StateRegionId,
                StateRegionCode = stateRegion.StateRegionCode,
                StateRegionNameEn = stateRegion.StateRegionNameEn,
                StateRegionNameMm = stateRegion.StateRegionNameMm
            };

            return Result<StateRegionResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting the state region requests for id " + id + " : " + ex.Message;
            _logger.LogError(message);
            return Result<StateRegionResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<StateRegionResponseModel>> UpdateAsync(int id, StateRegionRequestModel requestModel)
    {
        try
        {
            var stateRegion = await _context.Set<StateRegion>().FindAsync(id);
            if (stateRegion == null)
            {
                return Result<StateRegionResponseModel>.NotFoundError("State region with id " + id + " not found.");
            }

            stateRegion.StateRegionCode = requestModel.StateRegionCode;
            stateRegion.StateRegionNameEn = requestModel.StateRegionNameEn;
            stateRegion.StateRegionNameMm = requestModel.StateRegionNameMm;

            await _context.SaveChangesAsync();

            var model = new StateRegionResponseModel
            {
                StateRegionId = stateRegion.StateRegionId,
                StateRegionCode = stateRegion.StateRegionCode,
                StateRegionNameEn = stateRegion.StateRegionNameEn,
                StateRegionNameMm = stateRegion.StateRegionNameMm
            };

            return Result<StateRegionResponseModel>.Success(model, "State region updated successfully.");
        }
        catch (Exception ex)
        {
            string message = "An error occurred while updating the state region requests for id " + id + " : " + ex.Message;
            _logger.LogError(message);
            return Result<StateRegionResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var stateRegion = await _context.Set<StateRegion>().FindAsync(id);
        if (stateRegion == null)
            return Result<bool>.NotFoundError("State region not found.");

        _context.Remove(stateRegion);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true, "State region deleted successfully.");
    }

    public async Task<Result<List<StateRegionResponseModel>>> GetAllAsync()
    {
        var stateRegions = await _context.Set<StateRegion>().ToListAsync();

        var model = stateRegions.Select(sr => new StateRegionResponseModel
        {
            StateRegionId = sr.StateRegionId,
            StateRegionCode = sr.StateRegionCode,
            StateRegionNameEn = sr.StateRegionNameEn,
            StateRegionNameMm = sr.StateRegionNameMm
        }).ToList();

        return Result<List<StateRegionResponseModel>>.Success(model);
    }
}
