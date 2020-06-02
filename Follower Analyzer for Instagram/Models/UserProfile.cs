using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public User Owner { get; set; }
        public List<InstagramPost> Posts { get; set; }
    }
}