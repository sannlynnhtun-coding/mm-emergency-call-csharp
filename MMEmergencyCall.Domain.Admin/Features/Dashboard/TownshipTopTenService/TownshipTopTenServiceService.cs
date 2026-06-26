namespace MMEmergencyCall.Domain.Admin.Features.TownshipTopTenService;

public class TownshipTopTenServiceService
{
	private readonly AppDbContext _db;
	private readonly ILogger<TownshipTopTenServiceService> _logger;
	public TownshipTopTenServiceService(ILogger<TownshipTopTenServiceService> logger, AppDbContext db)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<Result<List<TownshipTopTenServiceResponseModel>>> GetTownshipTopTenService(string townshipCode)
	{
		try
		{
			var responseData = await _db.EmergencyRequests
				.Where(er => er.TownshipCode == townshipCode)
				.Join(
					_db.EmergencyServices,
					er => er.ServiceId,
					es => es.ServiceId,
					(er, es) => new { EmergencyRequest = er, EmergencyService = es })
				.GroupBy(x => new
				{
					x.EmergencyService.ServiceId,
					x.EmergencyService.UserId,
					x.EmergencyService.ServiceType,
					x.EmergencyService.ServiceName,
					x.EmergencyService.PhoneNumber,
					x.EmergencyService.Location,
					x.EmergencyService.Availability,
					x.EmergencyService.ServiceStatus
				})
				.Select(g => new TownshipTopTenServiceResponseModel
				{
					ServiceId = g.Key.ServiceId,
					UserId = g.Key.UserId,
					ServiceType = g.Key.ServiceType,
					ServiceName = g.Key.ServiceName,
					PhoneNumber = g.Key.PhoneNumber,
					Location = g.Key.Location,
					Availability = g.Key.Availability,
					ServiceStatus = g.Key.ServiceStatus,
					RequestCount = g.Count()
				})
				.OrderByDescending(x => x.RequestCount)
				.Take(10)
				.ToListAsync();

			_logger.LogInformation(responseData.ToString());

			return Result<List<TownshipTopTenServiceResponseModel>>.Success(responseData);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<List<TownshipTopTenServiceResponseModel>>.SystemError("Internal server error");
		}

	}
}
