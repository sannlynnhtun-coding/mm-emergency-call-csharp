namespace MMEmergencyCall.Domain.Client.Features.Register;

public class RegisterResponseModel
{
    public Result<RegisterModel> Result { get; set; }

    public RegisterResponseModel(Result<RegisterModel> result)
    {
        Result = result;
    }
}
