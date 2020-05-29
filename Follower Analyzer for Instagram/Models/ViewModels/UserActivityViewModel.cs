using System.Collections.Generic;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class UserActivityViewModel
    {
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public List<UserActivity> Activities { get; set; }

        public UserActivityViewModel()
        {
            Activities = new List<UserActivity>();
        }
    }
}