using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
{
    public class FollowersStatisticsViewModel
    {
        public List<User> NewFollowers { get; set; }
        public List<User> UnsubscribedFollowers { get; set; }
    }
}