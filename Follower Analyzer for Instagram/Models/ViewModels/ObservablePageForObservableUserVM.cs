using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ObservableAccountForObservableUserVM
    {
        [Required]
        public string TargetContentName { get; set; }
        [Required]
        public string observableUserName { get; set; }
        public List<SelectListItem> ObservableUsers { get; set; }
        public ObservableAccountForObservableUserVM()
        {
            ObservableUsers = new List<SelectListItem>();
        }
    }
}