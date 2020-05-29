using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class FollowerAnalyzerContext : DbContext
    {
        //public FollowerAnalyzerContext() : base("FollowerAnalyzerConnection") { }
        public FollowerAnalyzerContext() : base("FollowerAnalyzerExpressConnection") { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ObservableUser> ObservableUsers { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
    }
}