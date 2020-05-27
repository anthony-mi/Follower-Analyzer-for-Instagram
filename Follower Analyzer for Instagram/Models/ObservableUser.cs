using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class ObservableUser : User
    {
        public int Id { get; set; }
        public List<ObservableUser> ObservableAccaunts { get; set; }
        public ObservableUser()
        {
            ObservableAccaunts = new List<ObservableUser>();
        }
    }
}