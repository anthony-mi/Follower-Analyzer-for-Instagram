using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class InstagramPost
    {
        public string PrimaryKey { get; set; }
        public int CountOfLikes { get; set; }
        public List<User> Likers { get; set; }
        public int CountOfComments { get; set; }
        public List<User> Commenters { get; set; }
        public byte[] MediaFile { get; set; }
    }
}