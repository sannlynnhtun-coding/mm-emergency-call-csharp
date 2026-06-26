namespace MMEmergencyCall.Domain.Admin.Features.Townships;

public class TownshipService
{
    private readonly ILogger<TownshipService> _logger;
    private readonly AppDbContext _context;

    public TownshipService(ILogger<TownshipService> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<TownshipPaginationResponseModel>> GetAllAsync(int pageNo = 1, int pageSize = 10)
    {
        if (pageNo < 1 || pageSize < 1)
        {
            return Result<TownshipPaginationResponseModel>.BadRequestError("Invalid PageNo.");
        }

        var rowCount = await _context.Townships.CountAsync();
        int pageCount = (int)Math.Ceiling(rowCount / (double)pageSize);

        if (pageNo > pageCount && pageCount > 0)
        {
            return Result<TownshipPaginationResponseModel>.BadRequestError("Invalid PageNo.");
        }

        var townships = await _context
            .Townships.Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var lst = townships.Select(ts => new TownshipResponseModel
        {
            TownshipId = ts.TownshipId,
            TownshipCode = ts.TownshipCode,
            TownshipNameEn = ts.TownshipNameEn,
            TownshipNameMm = ts.TownshipNameMm,
            StateRegionCode = ts.StateRegionCode
        }).ToList();

        TownshipPaginationResponseModel model = new TownshipPaginationResponseModel();
        model.Data = lst;
        model.PageNo = pageNo;
        model.PageSize = pageSize;
        model.PageCount = pageCount;

        return Result<TownshipPaginationResponseModel>.Success(model);
    }

    public async Task<Result<TownshipResponseModel>> GetByIdAsync(int id)
    {
        try
        {
            var township = await _context.Townships.FindAsync(id);
            if (township is null)
                return Result<TownshipResponseModel>.NotFoundError("Township with id " + id + " not found.");

            var model = new TownshipResponseModel
            {
                TownshipId = township.TownshipId,
                TownshipCode = township.TownshipCode,
                TownshipNameEn = township.TownshipNameEn,
                TownshipNameMm = township.TownshipNameMm,
                StateRegionCode = township.StateRegionCode
            };

            return Result<TownshipResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting the township requests for id " + id + " : " + ex.Message;
            _logger.LogError(message);
            return Result<TownshipResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<TownshipResponseModel>> CreateTownshipAsync(TownshipRequestModel requestModel)
    {
        try
        {
            var township = new Township
            {
                TownshipCode = requestModel.TownshipCode,
                TownshipNameEn = requestModel.TownshipNameEn,
                TownshipNameMm = requestModel.TownshipNameMm,
                StateRegionCode = requestModel.StateRegionCode
            };

            _context.Add(township);
            await _context.SaveChangesAsync();

            var model = new TownshipResponseModel
            {
                TownshipId = township.TownshipId,
                TownshipCode = township.TownshipCode,
                TownshipNameEn = township.TownshipNameEn,
                TownshipNameMm = township.TownshipNameMm,
                StateRegionCode = township.StateRegionCode
            };

            return Result<TownshipResponseModel>.Success(model, "Township is created successfully.");
        }
        catch (Exception ex)
        {
            string message = "An error occurred while creating the township requests: " + ex.Message;
            _logger.LogError(message);
            return Result<TownshipResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<TownshipResponseModel>> UpdateTownshipAsync(int id, TownshipRequestModel requestModel)
    {
        try
        {
            var township = await _context.Townships.FindAsync(id);
            if (township is null)
                return Result<TownshipResponseModel>.NotFoundError("Township with id " + id + " not found.");

            township.TownshipCode = requestModel.TownshipCode;
            township.TownshipNameEn = requestModel.TownshipNameEn;
            township.TownshipNameMm = requestModel.TownshipNameMm;
            township.StateRegionCode = requestModel.StateRegionCode;

            _context.Entry(township).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var model = new TownshipResponseModel
            {
                TownshipId = township.TownshipId,
                TownshipCode = township.TownshipCode,
                TownshipNameEn = township.TownshipNameEn,
                TownshipNameMm = township.TownshipNameMm,
                StateRegionCode = township.StateRegionCode
            };

            return Result<TownshipResponseModel>.Success(model, "Township is updated successfully.");
        }
        catch (Exception ex)
        {
            string message = "An error occurred while updating the township requests for id " + id + " : " + ex.Message;
            _logger.LogError(message);
            return Result<TownshipResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var township = await _context.Townships.FindAsync(id);
        if (township is null)
            return Result<bool>.NotFoundError("Township with id " + id + " not found.");

        _context.Remove(township);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true, "Township is deleted successfully.");
    }
}
