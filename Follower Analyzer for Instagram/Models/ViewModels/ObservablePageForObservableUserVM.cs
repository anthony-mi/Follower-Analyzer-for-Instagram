using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
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