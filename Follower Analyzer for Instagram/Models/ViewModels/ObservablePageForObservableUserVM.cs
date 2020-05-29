using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ObservablePageForObservableUserVM
    {
        [Required]
        public string TargetContentName { get; set; }
        [Required]
        public string observableUserName { get; set; }
        public List<SelectListItem> ObservableUsers { get; set; }
        public ObservablePageForObservableUserVM()
        {
            ObservableUsers = new List<SelectListItem>();
        }
    }
}