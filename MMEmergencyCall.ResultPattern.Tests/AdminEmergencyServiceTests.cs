using MMEmergencyCall.Domain.Admin.Features.ApproveRejectEmergencyService;
using MMEmergencyCall.Domain.Admin.Features.CreateEmergencyService;
using MMEmergencyCall.Domain.Admin.Features.DeleteEmergencyService;
using MMEmergencyCall.Domain.Admin.Features.DeleteEmergencyServiceStatus;
using MMEmergencyCall.Domain.Admin.Features.EmergencyServiceList;
using MMEmergencyCall.Domain.Admin.Features.UpdateEmergencyService;
using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests;

public class AdminEmergencyServiceTests
{
    [Fact]
    public async Task EmergencyService_admin_create_list_update_and_delete_paths()
    {
        using var db = new TestDb();

        var created = ResultAssert.Success(await new CreateEmergencyServiceService(Logger.For<CreateEmergencyServiceService>(), db.Db)
            .CreateEmergencyServiceAsync(2, CreateRequest("Fire")));
        Assert.Equal(EnumServiceStatus.Pending.ToString(), created.ServiceStatus);

        var list = ResultAssert.Success(await new EmergencyServiceListService(Logger.For<EmergencyServiceListService>(), db.Db)
            .GetEmergencyServicesByStatusAsync("Pending"));
        Assert.Single(list.Data);
        ResultAssert.BadRequest(await new EmergencyServiceListService(Logger.For<EmergencyServiceListService>(), db.Db)
            .GetEmergencyServicesByStatusAsync(null, 0, 10));
        ResultAssert.BadRequest(await new EmergencyServiceListService(Logger.For<EmergencyServiceListService>(), db.Db)
            .GetEmergencyServicesByStatusAsync(null, 2, 10));
        ResultAssert.Validation(await new EmergencyServiceListService(Logger.For<EmergencyServiceListService>(), db.Db)
            .GetEmergencyServicesByStatusAsync("BadStatus"));

        Assert.True(ResultAssert.Success(await new ApproveRejectEmergencyServiceService(Logger.For<ApproveRejectEmergencyServiceService>(), db.Db)
            .UpdateEmergencyServiceStatusAsync(created.ServiceId, EnumServiceStatus.Approved.ToString())));
        ResultAssert.Validation(await new ApproveRejectEmergencyServiceService(Logger.For<ApproveRejectEmergencyServiceService>(), db.Db)
            .UpdateEmergencyServiceStatusAsync(created.ServiceId, "BadStatus"));
        ResultAssert.NotFound(await new ApproveRejectEmergencyServiceService(Logger.For<ApproveRejectEmergencyServiceService>(), db.Db)
            .UpdateEmergencyServiceStatusAsync(999, EnumServiceStatus.Approved.ToString()));

        var updated = ResultAssert.Success(await new UpdateEmergencyServiceService(Logger.For<UpdateEmergencyServiceService>(), db.Db)
            .UpdateEmergencyServiceAsync(created.ServiceId, UpdateRequest("Medical")));
        Assert.Equal("Medical", updated.ServiceType);
        ResultAssert.NotFound(await new UpdateEmergencyServiceService(Logger.For<UpdateEmergencyServiceService>(), db.Db)
            .UpdateEmergencyServiceAsync(999, UpdateRequest("Fire")));

        Assert.True(ResultAssert.Success(await new DeleteEmergencyServiceService(Logger.For<DeleteEmergencyServiceService>(), db.Db)
            .DeleteEmergencyServiceAsync(created.ServiceId)));
        ResultAssert.NotFound(await new DeleteEmergencyServiceService(Logger.For<DeleteEmergencyServiceService>(), db.Db)
            .DeleteEmergencyServiceAsync(999));

        Assert.True(ResultAssert.Success(await new DeleteEmergencyServiceStatusService(Logger.For<DeleteEmergencyServiceStatusService>(), db.Db)
            .DeleteEmergencyServiceStatusAsync(created.ServiceId)));
        ResultAssert.NotFound(await new DeleteEmergencyServiceStatusService(Logger.For<DeleteEmergencyServiceStatusService>(), db.Db)
            .DeleteEmergencyServiceStatusAsync(999));
    }

    [Fact]
    public async Task CreateEmergencyService_returns_system_error_when_context_fails()
    {
        var db = new TestDb();
        var service = new CreateEmergencyServiceService(Logger.For<CreateEmergencyServiceService>(), db.Db);
        db.Dispose();

        ResultAssert.SystemError(await service.CreateEmergencyServiceAsync(1, CreateRequest("Fire")));
    }

    private static CreateEmergencyServiceRequestModel CreateRequest(string type) => new()
    {
        ServiceGroup = "Emergency",
        ServiceType = type,
        ServiceName = $"{type} Service",
        PhoneNumber = "091111111",
        Location = "Yangon",
        Availability = "Y",
        TownshipCode = "TSH"
    };

    private static UpdateEmergencyServiceRequestModel UpdateRequest(string type) => new()
    {
        ServiceGroup = "Emergency",
        ServiceType = type,
        ServiceName = $"{type} Updated",
        PhoneNumber = "092222222",
        Location = "Yangon",
        Availability = "Y",
        TownshipCode = "TSH"
    };
}
