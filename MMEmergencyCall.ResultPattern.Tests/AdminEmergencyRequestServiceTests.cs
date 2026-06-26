using MMEmergencyCall.Domain.Admin.Features.CompleteCloseCancelEmergencyRequest;
using MMEmergencyCall.Domain.Admin.Features.EmergencyRequestList;
using MMEmergencyCall.Shared;

namespace MMEmergencyCall.ResultPattern.Tests;

public class AdminEmergencyRequestServiceTests
{
    [Fact]
    public async Task EmergencyRequest_list_returns_success_and_validation()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.Request());
        var service = new EmergencyRequestListService(Logger.For<EmergencyRequestListService>(), db.Db);

        Assert.Single(ResultAssert.Success(await service.GetEmergencyRequests(1, 10)).Data);
        ResultAssert.Validation(await service.GetEmergencyRequests(0, 10));
        ResultAssert.Validation(await service.GetEmergencyRequests(1, 0));
        ResultAssert.Validation(await service.GetEmergencyRequests(2, 10));
        ResultAssert.Validation(await service.GetEmergencyRequests(1, 10, status: "BadStatus"));
    }

    [Fact]
    public async Task CompleteCloseCancel_updates_status_and_returns_failures()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.Request());
        var service = new CompleteCloseCancelEmergencyRequestService(Logger.For<CompleteCloseCancelEmergencyRequestService>(), db.Db);

        Assert.True(ResultAssert.Success(await service.UpdateEmergencyRequestStatus(1, StatusRequest(EnumEmergencyRequestStatus.Completed))));
        ResultAssert.Validation(await service.UpdateEmergencyRequestStatus(1, new CompleteCloseCancelEmergencyRequestRequestModel { Status = "BadStatus" }));
        ResultAssert.NotFound(await service.UpdateEmergencyRequestStatus(999, StatusRequest(EnumEmergencyRequestStatus.Closed)));
    }

    private static CompleteCloseCancelEmergencyRequestRequestModel StatusRequest(EnumEmergencyRequestStatus status) => new()
    {
        Status = status.ToString()
    };
}
