using EnumServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus;

namespace MMEmergencyCall.Domain.Admin.Features.EmergencyServiceList;

public class EmergencyServiceListService
{
	private readonly ILogger<EmergencyServiceListService> _logger;

	private readonly AppDbContext _db;

	public EmergencyServiceListService(
		ILogger<EmergencyServiceListService> logger,
		AppDbContext db
	)
	{
		_logger = logger;
		_db = db;
	}

	public async Task<
		Result<EmergencyServicesListPaginationResponseModel>
	> GetEmergencyServicesByStatusAsync(string? status, int pageNo = 1, int pageSize = 10)
	{
		if (pageNo < 1 || pageSize < 1)
		{
			return Result<EmergencyServicesListPaginationResponseModel>.BadRequestError(
				"Invalid page number or page size"
			);
		}

		var serviceStatus = EnumServiceStatus.None;

		try
		{
			if (!status.IsNullOrEmpty())
			{
				if (!Enum.TryParse(status, true, out serviceStatus))
				{
					return Result<EmergencyServicesListPaginationResponseModel>.ValidationError(
						"Invalid Emergency Service Status. Status should be Pending, Approved, Rejected or Deleted"
					);
				}
			}

			var query = _db.EmergencyServices.AsQueryable();

			if (!serviceStatus.Equals(EnumServiceStatus.None))
			{
				query = query.Where(x => x.ServiceStatus == serviceStatus.ToString());
			}

			int rowCount = await query.CountAsync();
			int pageCount = (int)Math.Ceiling(rowCount / (double)pageSize);

			if (pageNo > pageCount && pageCount > 0)
			{
				return Result<EmergencyServicesListPaginationResponseModel>.BadRequestError(
					"Invalid page number"
				);
			}

			var lst = await query.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();

			var data = lst.Select(x => new EmergencyServiceListResponseModel
			{
				ServiceId = x.ServiceId,
				UserId = x.UserId,
				ServiceGroup = x.ServiceGroup,
				ServiceType = x.ServiceType,
				ServiceName = x.ServiceName,
				PhoneNumber = x.PhoneNumber,
				Location = x.Location,
				Availability = x.Availability,
				TownshipCode = x.TownshipCode,
				ServiceStatus = x.ServiceStatus
			})
				.ToList();

			var model = new EmergencyServicesListPaginationResponseModel()
			{
				Data = data,
				PageNo = pageNo,
				PageSize = pageSize,
				PageCount = pageCount
			};

			return Result<EmergencyServicesListPaginationResponseModel>.Success(model);
		}
		catch (ArgumentException ex)
		{
			_logger.LogError(ex.ToString());
			return Result<EmergencyServicesListPaginationResponseModel>.SystemError("Internal server error");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex.ToString());
			return Result<EmergencyServicesListPaginationResponseModel>.SystemError("Internal server error");
		}
	}
}
