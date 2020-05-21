using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.ViewModel
{
    public class PostViewModel
    {
        public string Headline { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string MediaPath { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
}