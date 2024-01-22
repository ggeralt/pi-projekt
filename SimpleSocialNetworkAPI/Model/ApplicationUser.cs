using Microsoft.AspNetCore.Identity;

namespace SimpleSocialNetworkAPI.Model
{
    public class ApplicationUser : IdentityUser
    {
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public List<Friendship> SentFriendRequests { get; set; }
        public List<Friendship> ReceivedFriendRequests { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
