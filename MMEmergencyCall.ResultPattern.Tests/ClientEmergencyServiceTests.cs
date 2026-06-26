using MMEmergencyCall.Domain.Client.Features.EmergencyServices;
using MMEmergencyCall.Domain.Client.Features.EmergencyServiceType;
using MMEmergencyCall.Shared;
using SharedServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus;

namespace MMEmergencyCall.ResultPattern.Tests;

public class ClientEmergencyServiceTests
{
    [Fact]
    public async Task EmergencyService_client_list_detail_create_update_delete_and_distance_paths()
    {
        using var db = new TestDb();
        await db.SaveAsync(
            Seed.Service(1, userId: 2, status: SharedServiceStatus.Approved.ToString(), serviceType: "Fire"),
            Seed.Service(2, userId: 2, status: SharedServiceStatus.Pending.ToString(), serviceType: "Medical"));
        var service = new EmergencyServiceService(Logger.For<EmergencyServiceService>(), db.Db);

        Assert.Single(ResultAssert.Success(await service.GetEmergencyServices(1, 10, null)).Data);
        ResultAssert.Validation(await service.GetEmergencyServices(0, 10, null));

        Assert.Equal(1, ResultAssert.Success(await service.GetEmergencyServiceById(1)).ServiceId);
        ResultAssert.NotFound(await service.GetEmergencyServiceById(999));

        var created = ResultAssert.Success(await service.CreateEmergencyServiceAsync(ServiceRequest("Rescue"), 2));
        Assert.Equal(SharedServiceStatus.Pending.ToString(), created.ServiceStatus);

        var updated = ResultAssert.Success(await service.UpdateEmergencyService(2, 2, ServiceRequest("Medical Updated")));
        Assert.Equal("Medical Updated", updated.ServiceType);
        ResultAssert.NotFound(await service.UpdateEmergencyService(999, 2, ServiceRequest("Missing")));
        ResultAssert.Validation(await service.UpdateEmergencyService(1, 999, ServiceRequest("Fire")));
        ResultAssert.Validation(await service.UpdateEmergencyService(1, 2, ServiceRequest("Fire")));

        Assert.True(ResultAssert.Success(await service.DeleteEmergencyService(2, 2)));
        ResultAssert.NotFound(await service.DeleteEmergencyService(999, 2));
        ResultAssert.Validation(await service.DeleteEmergencyService(1, 999));
        ResultAssert.InvalidData(await service.DeleteEmergencyService(1, 2));

        var nearby = ResultAssert.Success(await service.GetEmergencyServiceWithinDistanceAsync("TSH", "Fire", 16.8m, 96.15m, 5, 1, 10));
        Assert.Single(nearby.Data);
        ResultAssert.Validation(await service.GetEmergencyServiceWithinDistanceAsync(null, null, 0, 0, 0, 0, 10));
    }

    [Fact]
    public async Task EmergencyServiceType_returns_distinct_types_and_empty_success()
    {
        using var db = new TestDb();
        var service = new EmergencyServiceTypeService(Logger.For<EmergencyServiceTypeService>(), db.Db);
        Assert.Empty(ResultAssert.Success(await service.GetServiceTypesAsync()));

        await db.SaveAsync(Seed.Service(1, serviceType: "Fire"), Seed.Service(2, serviceType: "Fire"), Seed.Service(3, serviceType: "Medical"));
        var types = ResultAssert.Success(await service.GetServiceTypesAsync());
        Assert.Equal(2, types.Count);
    }

    [Fact]
    public async Task CreateEmergencyService_returns_system_error_when_context_fails()
    {
        var db = new TestDb();
        var service = new EmergencyServiceService(Logger.For<EmergencyServiceService>(), db.Db);
        db.Dispose();

        ResultAssert.SystemError(await service.CreateEmergencyServiceAsync(ServiceRequest("Fire"), 1));
    }

    private static EmergencyServiceRequestModel ServiceRequest(string type) => new()
    {
        ServiceGroup = "Emergency",
        ServiceType = type,
        ServiceName = $"{type} Service",
        PhoneNumber = "091111111",
        Location = "Yangon",
        Availability = "Y",
        TownshipCode = "TSH"
    };
}
