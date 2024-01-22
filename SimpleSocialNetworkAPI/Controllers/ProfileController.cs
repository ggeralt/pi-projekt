using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleSocialNetworkAPI.Services.Profile;

namespace SimpleSocialNetworkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("GetUserProfileById")]
        public async Task<IActionResult> GetProfileById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var result = await _profileService.GetProfileById(userId);

            if (result != null)
            {
                return Ok(result);
            }
            return null;
        }

        [HttpPut("ChangeProfileImage")]
        public async Task<IActionResult> ChangeProfileImage(string path, string userId)
        {
            if (string.IsNullOrEmpty(path))
            {
                return NotFound();
            }

            var result = await _profileService.ChangeProfileImage(path, userId);

            if (result != null)
            {
                return Ok(result);
            }
            return null;
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var result = await _profileService.DeleteUser(userId);

            if (result != null)
            {
                return Ok(result);
            }
            return null;
        }
    }
}
