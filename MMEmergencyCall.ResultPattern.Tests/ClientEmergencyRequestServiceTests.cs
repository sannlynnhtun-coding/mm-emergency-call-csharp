using MMEmergencyCall.Domain.Client.Features.EmergencyRequestById;
using MMEmergencyCall.Domain.Client.Features.EmergencyRequestHistory;
using MMEmergencyCall.Domain.Client.Features.EmergencyRequests;
using MMEmergencyCall.Domain.Client.Features.EmergencyServices;
using MMEmergencyCall.Domain.Client.Features.SubmitEmergencyRequest;
using MMEmergencyCall.Shared;
using SharedServiceStatus = MMEmergencyCall.Shared.EnumServiceStatus;

namespace MMEmergencyCall.ResultPattern.Tests;

public class ClientEmergencyRequestServiceTests
{
    [Fact]
    public async Task EmergencyRequest_service_returns_success_and_failures()
    {
        using var db = new TestDb();
        await db.SaveAsync(
            Seed.User(),
            Seed.Township(),
            Seed.Service(status: SharedServiceStatus.Approved.ToString()),
            Seed.Request());
        var service = new EmergencyRequestService(Logger.For<EmergencyRequestService>(), db.Db);

        Assert.Single(ResultAssert.Success(await service.GetEmergencyRequests(1, 10, userId: 1)).Data);
        ResultAssert.Validation(await service.GetEmergencyRequests(0, 10));
        ResultAssert.Validation(await service.GetEmergencyRequests(1, 0));
        ResultAssert.Validation(await service.GetEmergencyRequests(2, 10));
        ResultAssert.Validation(await service.GetEmergencyRequests(1, 10, status: "BadStatus"));

        Assert.Equal(1, ResultAssert.Success(await service.GetEmergencyRequestById(1, 1)).RequestId);
        ResultAssert.NotFound(await service.GetEmergencyRequestById(999, 1));

        var added = ResultAssert.Success(await service.AddEmergencyRequest(RequestModel(), 1));
        Assert.Equal(EnumEmergencyRequestStatus.Open.ToString(), added.Status);
        ResultAssert.Validation(await service.AddEmergencyRequest(RequestModel(), 999));

        var updated = ResultAssert.Success(await service.UpdateEmergencyRequestStatus(1, 1, StatusRequest(EnumEmergencyRequestStatus.Cancel)));
        Assert.Equal(EnumEmergencyRequestStatus.Cancel.ToString(), updated.Status);
        ResultAssert.Validation(await service.UpdateEmergencyRequestStatus(1, 1, new UpdateEmergencyRequestStatusRequest { Status = "BadStatus" }));
        ResultAssert.NotFound(await service.UpdateEmergencyRequestStatus(999, 1, StatusRequest(EnumEmergencyRequestStatus.Closed)));
    }

    [Fact]
    public async Task SubmitEmergencyRequest_returns_success_and_validation()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.User(), Seed.Township(), Seed.Service(status: SharedServiceStatus.Approved.ToString()));
        var service = new SubmitEmergencyRequestService(Logger.For<SubmitEmergencyRequestService>(), db.Db);

        var added = ResultAssert.Success(await service.AddEmergencyRequest(SubmitModel(), 1));
        Assert.Equal(EnumEmergencyRequestStatus.Open.ToString(), added.Status);
        ResultAssert.Validation(await service.AddEmergencyRequest(SubmitModel(), 999));
    }

    [Fact]
    public async Task RequestById_and_history_return_success_and_failures()
    {
        using var db = new TestDb();
        await db.SaveAsync(Seed.Request());

        var byId = new EmergencyRequestByIdService(Logger.For<EmergencyRequestByIdService>(), db.Db);
        Assert.Equal(1, ResultAssert.Success(await byId.GetEmergencyRequestById(1, 1)).RequestId);
        ResultAssert.NotFound(await byId.GetEmergencyRequestById(999, 1));

        var history = new EmergencyRequestHistoryService(Logger.For<EmergencyRequestHistoryService>(), db.Db);
        Assert.Single(ResultAssert.Success(await history.GetEmergencyRequests(1, 10, userId: 1)).Data);
        ResultAssert.Validation(await history.GetEmergencyRequests(0, 10));
        ResultAssert.Validation(await history.GetEmergencyRequests(1, 0));
        ResultAssert.Validation(await history.GetEmergencyRequests(2, 10));
        ResultAssert.Validation(await history.GetEmergencyRequests(1, 10, status: "BadStatus"));
    }

    private static EmergencyRequestRequestModel RequestModel() => new()
    {
        ServiceId = 1,
        ProviderId = 2,
        Notes = "Need help",
        TownshipCode = "TSH"
    };

    private static SubmitEmergencyRequestRequestModel SubmitModel() => new()
    {
        ServiceId = 1,
        ProviderId = 2,
        Notes = "Need help",
        TownshipCode = "TSH"
    };

    private static UpdateEmergencyRequestStatusRequest StatusRequest(EnumEmergencyRequestStatus status) => new()
    {
        Status = status.ToString()
    };
}
