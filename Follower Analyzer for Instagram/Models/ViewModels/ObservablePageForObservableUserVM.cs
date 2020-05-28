using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Models.ViewModels
{
    public class ObservablePageForObservableUserVM
    {
        public string TargetContentName { get; set; }
        public string observableUserName { get; set; }
        public List<SelectListItem> ObservableUsers { get; set; }
        public ObservablePageForObservableUserVM()
        {
            ObservableUsers = new List<SelectListItem>();
        }
    }
}