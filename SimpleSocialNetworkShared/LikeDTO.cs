using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocialNetworkShared
{
    public class LikeDTO
    {
        public ApplicationUserViewModel User { get; set; }
        public PostViewModel Post { get; set; }
    }
}
