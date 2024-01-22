using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkShared;

namespace SimpleSocialNetworkAPI.Services.Profile
{
    public interface IProfileService
    {
        Task<string> ChangeProfileImage(string path, string userId);
        Task<ResponseManager> DeleteUser(string userId);
        Task<ApplicationUserViewModel> GetProfileById(string id);
    }
}
