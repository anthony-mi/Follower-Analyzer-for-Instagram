using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ObservableUser : User
    {
        public int Id { get; set; }
        public virtual List<ObservableUser> ObservableUsers { get; set; }
        public virtual List<ObservableUser> ActivityInitiators { get; set; }
        public virtual List<ApplicationUser> Observers { get; set; }
        public ObservableUser()
        {
            Observers = new List<ApplicationUser>();
            ObservableUsers = new List<ObservableUser>();
            ActivityInitiators = new List<ObservableUser>();
        }
    }
}