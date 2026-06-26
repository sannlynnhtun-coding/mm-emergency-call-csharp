using Microsoft.Extensions.Logging;
using Geolocation;

namespace MMEmergencyCall.Domain.Client.Features.EmergencyServices;

public class EmergencyServiceService
{
    private readonly ILogger<EmergencyServiceService> _logger;

    private readonly AppDbContext _db;

    public EmergencyServiceService(ILogger<EmergencyServiceService> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<Result<EmergencyServicePaginationResponseModel>>
        GetEmergencyServices(int pageNo, int pageSize, string? serviceType)
    {
        if (pageNo < 1 || pageSize < 1)
        {
            return Result<EmergencyServicePaginationResponseModel>.ValidationError("Invalid PageNo.");
        }

        var query = _db.EmergencyServices
            .Where(x => x.ServiceStatus == EnumServiceStatus.Approved.ToString());

        if (!string.IsNullOrEmpty(serviceType))
        {
            query = query.Where(x => x.ServiceType == serviceType);
        }

        int totalRecords = await query.CountAsync();

        int pageCount = (int)Math.Ceiling(totalRecords / (double)pageSize);

        if (pageNo > pageCount && pageCount > 0)
        {
            return Result<EmergencyServicePaginationResponseModel>.ValidationError("Invalid PageNo.");
        }

        var emergencyService = await query
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

        var lst = emergencyService
            .Select(sr => new EmergencyServiceResponseModel
            {
                ServiceId = sr.ServiceId,
                UserId = sr.UserId,
                ServiceType = sr.ServiceType,
                ServiceGroup = sr.ServiceGroup,
                ServiceName = sr.ServiceName,
                PhoneNumber = sr.PhoneNumber,
                Location = sr.Location,
                Availability = sr.Availability,
                TownshipCode = sr.TownshipCode,
                ServiceStatus = sr.ServiceStatus
            })
            .ToList();

        EmergencyServicePaginationResponseModel model = new();
        model.Data = lst;
        model.PageSize = pageSize;
        model.PageNo = pageNo;
        model.PageCount = pageCount;

        return Result<EmergencyServicePaginationResponseModel>.Success(model);
    }

    public async Task<Result<EmergencyServiceResponseModel>> GetEmergencyServiceById(int serviceId)
    {
        try
        {
            var emergencyService = await _db.EmergencyServices.FirstOrDefaultAsync(x => x.ServiceId == serviceId);

            if (emergencyService is null)
            {
                return Result<EmergencyServiceResponseModel>
                    .NotFoundError("Emergency Service with Id: " + serviceId + " not found.");
            }

            var model = new EmergencyServiceResponseModel
            {
                ServiceId = emergencyService.ServiceId,
                UserId = emergencyService.UserId,
                ServiceGroup = emergencyService.ServiceGroup,
                ServiceType = emergencyService.ServiceType,
                ServiceName = emergencyService.ServiceName,
                PhoneNumber = emergencyService.PhoneNumber,
                Location = emergencyService.Location,
                Availability = emergencyService.Availability,
                TownshipCode = emergencyService.TownshipCode,
                ServiceStatus = emergencyService.ServiceStatus
            };

            return Result<EmergencyServiceResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while getting emergency service by ID for id "
                + serviceId + " : " + ex.ToString();
            _logger.LogError(message);
            return Result<EmergencyServiceResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<EmergencyServiceResponseModel>> CreateEmergencyServiceAsync(
        EmergencyServiceRequestModel request,
        int currentUserId)
    {
        try
        {
            var emergencyService = new EmergencyService
            {
                UserId = currentUserId,
                ServiceType = request.ServiceType,
                ServiceGroup = request.ServiceGroup,
                ServiceName = request.ServiceName,
                PhoneNumber = request.PhoneNumber,
                Location = request.Location,
                Availability = request.Availability,
                TownshipCode = request.TownshipCode,
                ServiceStatus = nameof(EnumServiceStatus.Pending)
            };

            _db.EmergencyServices.Add(emergencyService);
            await _db.SaveChangesAsync();

            var model = new EmergencyServiceResponseModel
            {
                ServiceId = emergencyService.ServiceId,
                UserId = emergencyService.UserId,
                ServiceGroup = emergencyService.ServiceGroup,
                ServiceType = emergencyService.ServiceType,
                ServiceName = emergencyService.ServiceName,
                PhoneNumber = emergencyService.PhoneNumber,
                Location = emergencyService.Location,
                Availability = emergencyService.Availability,
                TownshipCode = emergencyService.TownshipCode,
                ServiceStatus = emergencyService.ServiceStatus
            };

            return Result<EmergencyServiceResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message = "An error occurred while creating emergency service: " + ex.ToString();
            _logger.LogError(message);
            return Result<EmergencyServiceResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<EmergencyServiceResponseModel>> UpdateEmergencyService(
        int id,
        int currentUserId,
        EmergencyServiceRequestModel requestModel
    )
    {
        try
        {
            var emergencyService = await _db.Set<EmergencyService>().FindAsync(id);

            if (emergencyService is null)
            {
                return Result<EmergencyServiceResponseModel>
                    .NotFoundError("Emergency Service with id " + id + " not found.");
            }

            if (emergencyService.UserId != currentUserId)
            {
                return Result<EmergencyServiceResponseModel>
                    .ValidationError("You can edit only your own service.");
            }

            var status = emergencyService.ServiceStatus;
            if (status != "Pending")
            {
                return Result<EmergencyServiceResponseModel>
                    .ValidationError("You can edit only Services with Pending status.");
            }

            emergencyService.ServiceType = requestModel.ServiceType;
            emergencyService.ServiceGroup = requestModel.ServiceGroup;
            emergencyService.ServiceName = requestModel.ServiceName;
            emergencyService.PhoneNumber = requestModel.PhoneNumber;
            emergencyService.Location = requestModel.Location;
            emergencyService.Availability = requestModel.Availability;
            emergencyService.TownshipCode = requestModel.TownshipCode;
            emergencyService.ServiceStatus = nameof(EnumServiceStatus.Pending);

            _db.Entry(emergencyService).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var model = new EmergencyServiceResponseModel
            {
                ServiceId = emergencyService.ServiceId,
                UserId = emergencyService.UserId,
                ServiceType = emergencyService.ServiceType,
                ServiceGroup = emergencyService.ServiceGroup,
                ServiceName = emergencyService.ServiceName,
                PhoneNumber = emergencyService.PhoneNumber,
                Location = emergencyService.Location,
                Availability = emergencyService.Availability,
                TownshipCode = emergencyService.TownshipCode,
                ServiceStatus = emergencyService.ServiceStatus,
            };

            return Result<EmergencyServiceResponseModel>.Success(model);
        }
        catch (Exception ex)
        {
            string message =
                "An error occurred while updating the emergency service for id "
                + id
                + ": "
                + ex.Message;
            _logger.LogError(message);
            return Result<EmergencyServiceResponseModel>.SystemError("Internal server error");
        }
    }

    public async Task<Result<bool>> DeleteEmergencyService(int id, int currentUserId)
    {
        try
        {
            var emergencyService = await _db.EmergencyServices.FindAsync(id);

            if (emergencyService is null)
            {
                return Result<bool>.NotFoundError("Emergency Service not found.");
            }

            if (emergencyService.UserId != currentUserId)
            {
                return Result<bool>.ValidationError("You can delete only your own service.");
            }

            var status = emergencyService.ServiceStatus;
            if (status != "Pending")
            {
                return Result<bool>.InvalidDataError("You can delete only Services with Pending status.");
            }

            emergencyService.ServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus.Deleted.ToString();
            _db.Entry(emergencyService).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Result<bool>.Success(true, "Emergency Service deleted successfully.");
        }
        catch (Exception ex)
        {
            string message =
                "An error occurred while updating the emergency service for id "
                + id
                + ": "
                + ex.Message;
            _logger.LogError(message);
            return Result<bool>.SystemError("Internal server error");
        }
    }

    public Double ToRadians(decimal? angle)
    {
        var toRadians = 0.00;
        if (!String.IsNullOrEmpty(angle.ToString()))
        {
            toRadians = Convert.ToDouble(angle) * Math.PI / 180.0;
        }
        return toRadians;
    }

    public decimal CalculateDistance(decimal lat1, decimal lon1, decimal? lat2, decimal? lon2)
    {
        var distance = 0.00;
        var location1 = new Coordinate(52.2296756, 21.0122287);
        if (!String.IsNullOrEmpty(lat2.ToString()) && !String.IsNullOrEmpty(lon2.ToString()))
        {
            var r = 6371; // Radius of the Earth in mile
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            distance = r * c; // Distance in kilometers
        }

        return Convert.ToDecimal(distance);
    }

    public decimal CalculateDistanceByUsingLibrary(decimal lat1, decimal lon1, decimal? lat2, decimal? lon2)
    {
        //var distance = 0.00;
        var location1 = new Coordinate(Convert.ToDouble(lat1), Convert.ToDouble(lon1));
        var location2 = new Coordinate(Convert.ToDouble(lat2), Convert.ToDouble(lon2));
        var distance1 = GeoCalculator.GetDistance(location1, location2, 1); // 1 for kilometers
        
        return Convert.ToDecimal(distance1);
    }


    public async Task<Result<EmergencyServicesListWithDistance>> GetEmergencyServiceWithinDistanceAsync(string? townshipCode, string? emergencyType, decimal lat, decimal lng, decimal maxDistanceInKm, int pageNo, int pageSize)
    {

        if (pageNo < 1 || pageSize < 1)
        {
            return Result<EmergencyServicesListWithDistance>.ValidationError("Invalid PageNo.");
        }

        var query = _db.EmergencyServices
            .Where(x => x.ServiceStatus == EnumServiceStatus.Approved.ToString());

        if (!string.IsNullOrEmpty(townshipCode))
        {
            query = query.Where(x => x.TownshipCode != null && x.TownshipCode.ToUpper() == townshipCode.ToUpper());
        }
        if (!string.IsNullOrEmpty(emergencyType))
        {
            query = query.Where(x => x.ServiceType.ToUpper() == emergencyType.ToUpper());
        }

        var emergencyService = await query.ToListAsync();

        List<EmergencyServicesWithDistance> emergencyServicesWithinDistance = new List<EmergencyServicesWithDistance>();
        if (!string.IsNullOrEmpty(lat.ToString()) && !string.IsNullOrEmpty(lng.ToString())
            && maxDistanceInKm > 0)

        {
            // Calculate distance and filter locations
            emergencyServicesWithinDistance = emergencyService
               .Select(emergencyServices => new EmergencyServicesWithDistance
               {
                   ServiceId = emergencyServices.ServiceId,
                   UserId = emergencyServices.UserId,
                   ServiceGroup = emergencyServices.ServiceGroup,
                   ServiceType = emergencyServices.ServiceType,
                   ServiceName = emergencyServices.ServiceName,
                   PhoneNumber = emergencyServices.PhoneNumber,
                   Location = emergencyServices.Location,
                   Availability = emergencyServices.Availability,
                   TownshipCode = emergencyServices.TownshipCode,
                   ServiceStatus = emergencyServices.ServiceStatus,
                   Ltd = emergencyServices.Ltd,
                   Lng = emergencyServices.Lng,
                   Distance = CalculateDistanceByUsingLibrary(lat, lng, emergencyServices.Ltd, emergencyServices.Lng)
               })
               .Where(location => location.Distance <= maxDistanceInKm)
               .OrderBy(location => location.Distance)
               .ToList();
        }
        else
        {
            emergencyServicesWithinDistance = emergencyService
           .Select(emergencyServices => new EmergencyServicesWithDistance
           {
               ServiceId = emergencyServices.ServiceId,
               UserId = emergencyServices.UserId,
               ServiceGroup = emergencyServices.ServiceGroup,
               ServiceType = emergencyServices.ServiceType,
               ServiceName = emergencyServices.ServiceName,
               PhoneNumber = emergencyServices.PhoneNumber,
               Location = emergencyServices.Location,
               Availability = emergencyServices.Availability,
               TownshipCode = emergencyServices.TownshipCode,
               ServiceStatus = emergencyServices.ServiceStatus,
               Ltd = emergencyServices.Ltd,
               Lng = emergencyServices.Lng,
               Distance = 0
           })
       .ToList();
        }

        int totalRecords = emergencyServicesWithinDistance.Count;
        int pageCount = (int)Math.Ceiling(totalRecords / (double)pageSize);

        if (pageNo > pageCount && pageCount > 0)
        {
            return Result<EmergencyServicesListWithDistance>.ValidationError("Invalid PageNo.");
        }

        EmergencyServicesListWithDistance model = new();
        model.PageNo = pageNo;
        model.PageSize = pageSize;
        model.PageCount = pageCount;
        model.Data = emergencyServicesWithinDistance
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<EmergencyServicesListWithDistance>.Success(model);

    }

}
