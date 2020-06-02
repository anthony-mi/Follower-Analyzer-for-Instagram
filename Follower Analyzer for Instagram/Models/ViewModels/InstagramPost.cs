using System.Collections.Generic;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class InstagramPost
    {
        public string PrimaryKey { get; set; }
        public int CountOfLikes { get; set; }
        public  List<User> Likers { get; set; }
        public int CountOfComments { get; set; }
        public List<User> Commenters { get; set; }
        public string MediaFileUri { get; set; }
        public InstagramPost()
        {
            Likers = new List<User>();
            Commenters = new List<User>();
        }
    }
}