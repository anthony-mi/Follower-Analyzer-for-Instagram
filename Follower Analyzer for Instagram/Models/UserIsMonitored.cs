using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class UserIsMonitored
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual ICollection<InstagramPost> Posts { get; set; }
    }
}