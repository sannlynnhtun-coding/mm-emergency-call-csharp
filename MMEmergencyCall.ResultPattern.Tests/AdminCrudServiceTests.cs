using MMEmergencyCall.Domain.Admin.Features.CreateUser;
using MMEmergencyCall.Domain.Admin.Features.DeleteUser;
using MMEmergencyCall.Domain.Admin.Features.StateRegions;
using MMEmergencyCall.Domain.Admin.Features.Townships;
using MMEmergencyCall.Domain.Admin.Features.UpdateUser;
using MMEmergencyCall.Domain.Admin.Features.UserById;
using MMEmergencyCall.Domain.Admin.Features.UserList;

namespace MMEmergencyCall.ResultPattern.Tests;

public class AdminCrudServiceTests
{
    [Fact]
    public async Task StateRegion_crud_returns_success_and_not_found()
    {
        using var db = new TestDb();
        var service = new StateRegionService(Logger.For<StateRegionService>(), db.Db);

        var created = ResultAssert.Success(await service.CreateAsync(StateRegionRequest("SR1")));
        Assert.Equal("SR1", created.StateRegionCode);
        Assert.Single(ResultAssert.Success(await service.GetAllAsync()));
        Assert.Equal(created.StateRegionId, ResultAssert.Success(await service.GetByIdAsync(created.StateRegionId)).StateRegionId);

        var updated = ResultAssert.Success(await service.UpdateAsync(created.StateRegionId, StateRegionRequest("SR2")));
        Assert.Equal("SR2", updated.StateRegionCode);
        Assert.True(ResultAssert.Success(await service.DeleteAsync(created.StateRegionId)));

        ResultAssert.NotFound(await service.GetByIdAsync(created.StateRegionId));
        ResultAssert.NotFound(await service.UpdateAsync(999, StateRegionRequest("SR3")));
        ResultAssert.NotFound(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task Township_crud_and_pagination_return_success_and_failures()
    {
        using var db = new TestDb();
        var service = new TownshipService(Logger.For<TownshipService>(), db.Db);

        ResultAssert.BadRequest(await service.GetAllAsync(0, 10));
        var created = ResultAssert.Success(await service.CreateTownshipAsync(TownshipRequest("TSH1")));
        Assert.Equal("TSH1", created.TownshipCode);
        Assert.Single(ResultAssert.Success(await service.GetAllAsync()).Data);
        Assert.Equal(created.TownshipId, ResultAssert.Success(await service.GetByIdAsync(created.TownshipId)).TownshipId);

        Assert.Equal("TSH2", ResultAssert.Success(await service.UpdateTownshipAsync(created.TownshipId, TownshipRequest("TSH2"))).TownshipCode);
        Assert.True(ResultAssert.Success(await service.DeleteAsync(created.TownshipId)));

        ResultAssert.NotFound(await service.GetByIdAsync(created.TownshipId));
        ResultAssert.NotFound(await service.UpdateTownshipAsync(999, TownshipRequest("TSH3")));
        ResultAssert.NotFound(await service.DeleteAsync(999));
    }

    [Fact]
    public async Task User_services_return_success_and_main_failures()
    {
        using var db = new TestDb();

        var created = ResultAssert.Success(await new CreateUserService(Logger.For<CreateUserService>(), db.Db)
            .CreateUserAsync(UserCreateRequest("user@gmail.com")));
        Assert.Equal("user@gmail.com", created.Email);

        var users = ResultAssert.Success(await new UserListService(Logger.For<UserListService>(), db.Db)
            .GetUsersAsync(1, 10, null, null));
        Assert.Single(users.Data);
        ResultAssert.Validation(await new UserListService(Logger.For<UserListService>(), db.Db)
            .GetUsersAsync(0, 10, null, null));

        Assert.Equal(created.UserId, ResultAssert.Success(await new UserByIdService(Logger.For<UserByIdService>(), db.Db)
            .GetByIdAsync(created.UserId)).UserId);
        ResultAssert.NotFound(await new UserByIdService(Logger.For<UserByIdService>(), db.Db).GetByIdAsync(999));

        var updated = ResultAssert.Success(await new UpdateUserService(Logger.For<UpdateUserService>(), db.Db)
            .UpdateUserAsync(created.UserId, UserUpdateRequest("updated@gmail.com")));
        Assert.Equal("updated@gmail.com", updated.Email);
        ResultAssert.NotFound(await new UpdateUserService(Logger.For<UpdateUserService>(), db.Db)
            .UpdateUserAsync(999, UserUpdateRequest("missing@gmail.com")));

        db.Db.ChangeTracker.Clear();
        Assert.True(ResultAssert.Success(await new DeleteUserService(Logger.For<DeleteUserService>(), db.Db)
            .DeleteUserAsync(created.UserId)));
        ResultAssert.NotFound(await new DeleteUserService(Logger.For<DeleteUserService>(), db.Db).DeleteUserAsync(999));
    }

    [Fact]
    public async Task CreateUser_returns_system_error_when_context_fails()
    {
        var db = new TestDb();
        var service = new CreateUserService(Logger.For<CreateUserService>(), db.Db);
        db.Dispose();

        ResultAssert.SystemError(await service.CreateUserAsync(UserCreateRequest("boom@gmail.com")));
    }

    private static StateRegionRequestModel StateRegionRequest(string code) => new()
    {
        StateRegionCode = code,
        StateRegionNameEn = "State",
        StateRegionNameMm = "State MM"
    };

    private static TownshipRequestModel TownshipRequest(string code) => new()
    {
        TownshipCode = code,
        TownshipNameEn = "Township",
        TownshipNameMm = "Township MM",
        StateRegionCode = "SR"
    };

    private static CreateUserRequestModel UserCreateRequest(string email) => new()
    {
        Name = "User",
        Email = email,
        Password = "pass",
        PhoneNumber = "099999999",
        Address = "Yangon",
        TownshipCode = "TSH",
        Role = "Normal User",
        EmergencyType = "Fire",
        EmergencyDetails = "Details"
    };

    private static UpdateUserRequestModel UserUpdateRequest(string email) => new()
    {
        Name = "Updated",
        Email = email,
        PhoneNumber = "098888888",
        Address = "Mandalay",
        TownshipCode = "TSH",
        Role = "Normal User",
        UserStatus = "Activated",
        EmergencyType = "Medical",
        EmergencyDetails = "Updated"
    };
}
