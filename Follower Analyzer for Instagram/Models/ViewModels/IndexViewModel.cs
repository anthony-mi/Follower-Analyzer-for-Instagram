using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
{
    public class IndexViewModel
    {
        [Required]
        public string Username { get; set; }

        public List<InstagramPost> Posts { get; set; }
    }
}