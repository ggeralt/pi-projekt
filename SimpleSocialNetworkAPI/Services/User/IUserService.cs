using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkShared;

namespace SimpleSocialNetworkAPI.Services.User
{
    public interface IUserService
    {
        Task<ResponseManager> RegisterUser(RegisterDTO Dto);
        Task<ResponseManager> LoginUser(LoginDTO loginDTO);
        Task<ResponseManager> ConfirmEmail(string userId, string token);
        Task<ResponseManager> ForgetPassword(string email);
        Task<ResponseManager> ResetPassword(ResetPassword model);
    }
}
