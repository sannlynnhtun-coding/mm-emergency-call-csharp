using Microsoft.Extensions.Logging;
using MMEmergencyCall.Domain.Client.Features.EmergencyServices;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyRequests;

public class EmergencyRequestService
{
    private readonly ILogger<EmergencyRequestService> _logger;
    private readonly AppDbContext _db;

    public EmergencyRequestService(ILogger<EmergencyRequestService> logger, AppDbContext context)
    {
        _logger = logger;
        _db = context;
    }

    public async Task<Result<EmergencyRequestPaginationResponseModel>> GetEmergencyRequests(int pageNo, int pageSize,
     int? userId = null, string? serviceId = null, string? providerId = null,
     string? status = null, string? townshipCode = null)
    {
        try
        {
            if (pageNo < 1 || pageSize < 1)
            {
                return Result<EmergencyRequestPaginationResponseModel>.ValidationError("Invalid PageNo.");
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
                    return Result<EmergencyRequestPaginationResponseModel>.ValidationError(
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
                return Result<EmergencyRequestPaginationResponseModel>.ValidationError("Invalid PageNo.");
            }

            var emergencyRequests = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var responseData = emergencyRequests.Select(x => new EmergencyRequestResponseModel
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

            var model = new EmergencyRequestPaginationResponseModel
            {
                PageNo = pageNo,
                PageSize = pageSize,
                PageCount = pageCount,
                Data = responseData
            };

            return Result<EmergencyRequestPaginationResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting the emergency requests: " + ex.Message;
            _logger.LogError(message);
            return Result<EmergencyRequestPaginationResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<EmergencyRequestResponseModel>> GetEmergencyRequestById(int id, int? userId)
    {
        try
        {
            var emergencyRequest = await _db.EmergencyRequests.Where(x => x.RequestId == id && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (emergencyRequest is null)
            {
                return Result<EmergencyRequestResponseModel>
                        .NotFoundError("Emergency Request with Id " + id + " not found.");
            }

            var model = new EmergencyRequestResponseModel()
            {
                RequestId = emergencyRequest.RequestId,
                UserId = emergencyRequest.UserId,
                ServiceId = emergencyRequest.ServiceId,
                ProviderId = emergencyRequest.ProviderId,
                RequestTime = emergencyRequest.RequestTime,
                Status = emergencyRequest.Status,
                ResponseTime = emergencyRequest.ResponseTime,
                Notes = emergencyRequest.Notes,
                TownshipCode = emergencyRequest.TownshipCode
            };

            return Result<EmergencyRequestResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting the emergency request with id " + id + " : " + ex.Message;
            _logger.LogError(message);
            return Result<EmergencyRequestResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<EmergencyRequestResponseModel>> AddEmergencyRequest(EmergencyRequestRequestModel request, int currentUserId)
    {
        try
        {
            if (request is null)
            {
                return Result<EmergencyRequestResponseModel>
                    .ValidationError("Request model cannot be null.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(x => x.UserId == currentUserId);
            if (user is null)
            {
                return Result<EmergencyRequestResponseModel>
                    .ValidationError("Invalid User");
            }

            var townshipCode = string.IsNullOrEmpty(request.TownshipCode)
                ? user.TownshipCode
                : request.TownshipCode;

            var validateRequestModelResponse = await ValidateEmergencyRequestRequestModel(request, townshipCode);

            if (validateRequestModelResponse is not null)
            {
                return validateRequestModelResponse;
            }

            var emergencyRequest = new EmergencyRequest()
            {
                UserId = currentUserId,
                ServiceId = request.ServiceId,
                ProviderId = request.ProviderId,
                RequestTime = DateTime.Now,
                Status = nameof(EnumEmergencyRequestStatus.Open),
                ResponseTime = null,
                Notes = request.Notes,
                TownshipCode = townshipCode
            };

            _db.EmergencyRequests.Add(emergencyRequest);
            await _db.SaveChangesAsync();

            var model = new EmergencyRequestResponseModel()
            {
                RequestId = emergencyRequest.RequestId,
                UserId = emergencyRequest.UserId,
                ServiceId = emergencyRequest.ServiceId,
                ProviderId = emergencyRequest.ProviderId,
                RequestTime = emergencyRequest.RequestTime,
                Status = emergencyRequest.Status,
                ResponseTime = emergencyRequest.ResponseTime,
                Notes = emergencyRequest.Notes,
                TownshipCode = emergencyRequest.TownshipCode
            };

            return Result<EmergencyRequestResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while adding the emergency request : " + ex.Message;
            _logger.LogError(message);
            return Result<EmergencyRequestResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<EmergencyRequestResponseModel>> UpdateEmergencyRequestStatus(int id, int userId, UpdateEmergencyRequestStatusRequest statusRequest)
    {
        try
        {
            if (!Enum.IsDefined(typeof(EnumEmergencyRequestStatus), statusRequest.Status))
            {
                return Result<EmergencyRequestResponseModel>.ValidationError(
                    "Invalid Emergency Request Status. Status should be Open, Cancel, Closed or Completed"
                );
            }

            if (!await IsUserIdExist(userId))
            {
                return Result<EmergencyRequestResponseModel>
                    .ValidationError("Invalid User");
            }

            var existingEmergencyRequest = await _db.EmergencyRequests
                .FirstOrDefaultAsync(x => x.RequestId == id && x.UserId == userId);

            if (existingEmergencyRequest is null)
            {
                return Result<EmergencyRequestResponseModel>
                      .NotFoundError("Emergency Request with Id " + id + " not found.");
            }

            existingEmergencyRequest.Status = statusRequest.Status;
            _db.Entry(existingEmergencyRequest).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var model = new EmergencyRequestResponseModel()
            {
                RequestId = existingEmergencyRequest.RequestId,
                UserId = existingEmergencyRequest.UserId,
                ServiceId = existingEmergencyRequest.ServiceId,
                ProviderId = existingEmergencyRequest.ProviderId,
                RequestTime = existingEmergencyRequest.RequestTime,
                Status = existingEmergencyRequest.Status,
                ResponseTime = existingEmergencyRequest.ResponseTime,
                Notes = existingEmergencyRequest.Notes,
                TownshipCode = existingEmergencyRequest.TownshipCode
            };

            return Result<EmergencyRequestResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while updating the status of emergency request with id " + id + " : " +
                             ex.Message;
            _logger.LogError(message);
            return Result<EmergencyRequestResponseModel>.SystemError("Internal server error");
        }
    }

    #region privateMethods
    private async Task<Result<EmergencyRequestResponseModel>> ValidateEmergencyRequestRequestModel
        (EmergencyRequestRequestModel? request, string? townshipCode)
    {
        if (request is null)
        {
            return Result<EmergencyRequestResponseModel>
                .ValidationError("Request model cannot be null.");
        }

        if (request.ProviderId.HasValue && request.ProviderId < 1)
        {
            return Result<EmergencyRequestResponseModel>
                .ValidationError("Invalid Provider Id.");
        }

        if (!await IsApprovedServiceIdExist(request.ServiceId))
        {
            return Result<EmergencyRequestResponseModel>
                .ValidationError("Invalid or unapproved Service Id.");
        }

        if (!string.IsNullOrEmpty(townshipCode) && !await IsTownshipCodeExist(townshipCode))
        {
            return Result<EmergencyRequestResponseModel>
                .ValidationError("Invalid Township Code.");
        }

        return null;
    }

    private async Task<bool> IsUserIdExist(int userId)
    {
        var isUserIdExist = await _db.Users.AnyAsync(x => x.UserId == userId);
        return isUserIdExist;
    }

    //private async Task<bool> IsProviderIdExist(int providerId)
    //{
    //    var isProviderIdExist = await _db.
    //}

    private async Task<bool> IsApprovedServiceIdExist(int serviceId)
    {
        var isServiceIdExist = await _db.EmergencyServices.AnyAsync(x =>
            x.ServiceId == serviceId &&
            x.ServiceStatus == MMEmergencyCall.Shared.EnumServiceStatus.Approved.ToString());
        return isServiceIdExist;
    }

    private async Task<bool> IsTownshipCodeExist(string townshipCode)
    {
        var isTownshipCodeExist = await _db.Townships.AnyAsync(x => x.TownshipCode == townshipCode);
        return isTownshipCodeExist;
    }
    #endregion
}
