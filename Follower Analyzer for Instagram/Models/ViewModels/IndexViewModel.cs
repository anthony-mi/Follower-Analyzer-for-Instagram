using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class IndexViewModel
    {
        [Required]
        public string Username { get; set; }

        public List<InstagramPost> Posts { get; set; }
    }
}