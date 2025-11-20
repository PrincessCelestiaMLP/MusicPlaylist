using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music
{
    public class Follow
    {
        public string Id { get; set; }
        public UserView Follower { get; set; }
        public DateTime FollowedAt { get; set; }
    }
}
