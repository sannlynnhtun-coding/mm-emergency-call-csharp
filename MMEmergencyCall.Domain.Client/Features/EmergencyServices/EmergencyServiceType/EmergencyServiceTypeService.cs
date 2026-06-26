using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyServiceType;

public class EmergencyServiceTypeService
{
    private readonly ILogger<EmergencyServiceTypeService> _logger;
    private readonly AppDbContext _db;

    public EmergencyServiceTypeService(
        ILogger<EmergencyServiceTypeService> logger,
        AppDbContext context
    )
    {
        _logger = logger;
        _db = context;
    }

    public async Task<Result<List<string>>> GetServiceTypesAsync()
    {
        try
        {
            List<string> lst = await _db
                .EmergencyServices.Select(s => s.ServiceType)
                .Distinct()
                .ToListAsync();

            if (lst is null)
            {
                string message = "There is no emergency service types found in the database";
                return Result<List<string>>.NotFoundError(message);
            }

            var model = Result<List<string>>.Success(lst);
            return model;
        }
        catch (Exception ex)
        {
            string message =
                "An error occurred while getting emergency service types." + ex.ToString();
            _logger.LogError(message);
            return Result<List<string>>.SystemError("Internal server error");
        }
    }
}
