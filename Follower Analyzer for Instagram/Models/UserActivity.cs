using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public enum UserActivityType
    {
        Like,
        Comment
    }
    public class UserActivity
    {
        public int Id { get; set; }
        public string TargetUserName { get; set; }
        public string InitiatorPrimaryKey { get; set; }
        public DateTime EventDate { get; set; }
        public string LinkToMedia { get; set; }
        public UserActivityType ActivityType { get; set; }
    }
}