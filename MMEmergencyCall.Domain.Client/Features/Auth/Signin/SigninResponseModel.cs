namespace MMEmergencyCall.Domain.Client.Features.Signin;

public class SigninResponseModel
{
    public Result<SigninModel> Result { get; set; }

    public SigninResponseModel(Result<SigninModel> result)
    {
        Result = result;
    }
}