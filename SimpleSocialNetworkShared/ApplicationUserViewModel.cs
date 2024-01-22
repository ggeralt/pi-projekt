using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleSocialNetworkShared
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string ProfilePicture { get; set; }
        public List<PostViewModel>? Posts { get; set; }

        //public List<Post> Posts { get; set; }
        //public List<Comment> Comments { get; set; }
        //public List<Like> Likes { get; set; }
        //public List<Friendship> SentFriendRequests { get; set; }
        //public List<Friendship> ReceivedFriendRequests { get; set; }
    }
}
