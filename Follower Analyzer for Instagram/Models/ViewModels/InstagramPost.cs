using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class InstagramPost
    {
        [Key]
        public string PrimaryKey { get; set; }
        public int CountOfLikes { get; set; }
        public  List<ObservableUser> Likers { get; set; }
        public int CountOfComments { get; set; }
        public List<ObservableUser> Commenters { get; set; }
        public string MediaFileUri { get; set; }
        public InstagramPost()
        {
            Likers = new List<ObservableUser>();
            Commenters = new List<ObservableUser>();
        }
    }
}