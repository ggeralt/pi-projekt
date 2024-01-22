using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleSocialNetworkAPI.Model;
using SimpleSocialNetworkShared;
using System.Security.Claims;

namespace SimpleSocialNetworkAPI.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public ProfileService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<string> ChangeProfileImage(string path, string userId)
        {
            var CurrentUserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(CurrentUserId);
            if (user == null || user.Id != userId)
            {
                return null;
            }
            string oldProfilePicture = user.ProfilePicture;
            user.ProfilePicture = path;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return oldProfilePicture;
            }
            return null;
        }

        public async Task<ResponseManager> DeleteUser(string userId)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != userId)
            {
                return new ResponseManager
                {
                    Message = "User can not be deleted",
                    IsSuccess = false
                };
            }
            var user = await _userManager.Users.Include(x => x.Posts).Include(x => x.Comments).Include(x => x.Likes).SingleAsync(user => user.Id == userId);
            if (user == null)
            {
                return new ResponseManager
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }
            _context.Posts.RemoveRange(user.Posts);
            _context.Comments.RemoveRange(user.Comments);
            _context.Likes.RemoveRange(user.Likes);
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return new ResponseManager
                {
                    Message = "User deleted successfully",
                    IsSuccess = true
                };
            }
            return new ResponseManager
            {
                Message = "User can not be deleted",
                IsSuccess = false
            };
        }

        public async Task<ApplicationUserViewModel> GetProfileById(string id)
        {
            var user = await _userManager.Users.Include(x => x.Posts).SingleAsync(user => user.Id == id);
            if (user == null)
            {
                return null;
            }
            List<PostViewModel> posts = new List<PostViewModel>();
            foreach (var post in user.Posts)
            {
                posts.Add(new PostViewModel
                {
                    Id = post.Id,
                    MediaPath = post.MediaPath,
                    Content = post.Content,
                });
            }
            ApplicationUserViewModel userViewModel = new ApplicationUserViewModel
            {
                Id = user.Id,
                ProfilePicture = user.ProfilePicture,
                Username = user.UserName,
                Posts = posts,
            };
            return userViewModel;
        }
    }
}
