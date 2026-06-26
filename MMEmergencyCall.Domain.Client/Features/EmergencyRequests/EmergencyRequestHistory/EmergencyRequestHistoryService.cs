using Microsoft.Extensions.Logging;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequestHistory;

public class EmergencyRequestHistoryService
{
	private readonly ILogger<EmergencyRequestHistoryService> _logger;
	private readonly AppDbContext _db;

	public EmergencyRequestHistoryService(
		ILogger<EmergencyRequestHistoryService> logger, 
		AppDbContext context
		)
	{
		_logger = logger;
		_db = context;
	}

	public async Task<Result<EmergencyRequestHistoryPaginationResponseModel>> GetEmergencyRequests(int pageNo, int pageSize,
	 int? userId = null, string? serviceId = null, string? providerId = null,
	 string? status = null, string? townshipCode = null)
	{
		try
		{
			if (pageNo < 1 || pageSize < 1)
			{
				return Result<EmergencyRequestHistoryPaginationResponseModel>.ValidationError("Invalid PageNo.");
			}

			var query = _db.EmergencyRequests.AsQueryable();

			if (userId.HasValue)
			{
				query = query.Where(x => x.UserId == userId.Value);
			}
			if (!string.IsNullOrEmpty(serviceId))
			{
				query = query.Where(x => x.ServiceId.ToString() == serviceId);
			}
			if (!string.IsNullOrEmpty(providerId))
			{
				query = query.Where(x => x.ProviderId.ToString() == providerId);
			}

			if (!string.IsNullOrEmpty(status))
			{
				if (!Enum.IsDefined(typeof(EnumEmergencyRequestStatus), status))
				{
					return Result<EmergencyRequestHistoryPaginationResponseModel>.ValidationError(
						"Invalid Emergency Request Status. Status should be Open, Cancel, Closed or Completed"
					);
				}

				query = query.Where(x => x.Status == status);
			}

			if (!string.IsNullOrEmpty(status))
			{
				query = query.Where(x => x.Status == status);
			}
			if (!string.IsNullOrEmpty(townshipCode))
			{
				query = query.Where(x => x.TownshipCode == townshipCode);
			}

			int totalRecords = await query.CountAsync();

			int pageCount = (int)Math.Ceiling(totalRecords / (double)pageSize);

			if (pageNo > pageCount && pageCount > 0)
			{
				return Result<EmergencyRequestHistoryPaginationResponseModel>.ValidationError("Invalid PageNo.");
			}

			var emergencyRequests = await query
				.Skip((pageNo - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var responseData = emergencyRequests.Select(x => new EmergencyRequestHistoryResponseModel
			{
				RequestId = x.RequestId,
				UserId = x.UserId,
				ServiceId = x.ServiceId,
				ProviderId = x.ProviderId,
				RequestTime = x.RequestTime,
				Status = x.Status,
				ResponseTime = x.ResponseTime,
				Notes = x.Notes,
				TownshipCode = x.TownshipCode
			}).ToList();

			var model = new EmergencyRequestHistoryPaginationResponseModel
			{
				PageNo = pageNo,
				PageSize = pageSize,
				PageCount = pageCount,
				Data = responseData
			};

			return Result<EmergencyRequestHistoryPaginationResponseModel>.Success(model);
		}
		catch (Exception ex)
		{
			string message = "An error occurred while getting the emergency requests: " + ex.Message;
			_logger.LogError(message);
			return Result<EmergencyRequestHistoryPaginationResponseModel>.SystemError("Internal server error");
		}
	}
}
