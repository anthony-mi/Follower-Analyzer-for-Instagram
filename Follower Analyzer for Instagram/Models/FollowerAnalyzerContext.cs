using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Follower_Analyzer_for_Instagram.Models
{
    public class FollowerAnalyzerContext : DbContext
    {
        public FollowerAnalyzerContext() : base("FollowerAnalyzerExpressConnection") { }

        public DbSet<User> Users { get; set; }
    }
}