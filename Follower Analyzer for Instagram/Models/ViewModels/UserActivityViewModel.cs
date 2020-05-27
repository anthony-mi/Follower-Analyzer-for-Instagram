using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
{
    public class UserActivityViewModel
    {
        public string ProfilePictureUrl { get; set; }
        public string UserName { get; set; }
        public List<UserActivity> Activities { get; set; }
    }
}