namespace MMEmergencyCall.Domain.Client.Features.Logout;

public class LogoutResponseModel
{
    public bool IsSuccess { get; set; }

    public LogoutResponseModel(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
}
