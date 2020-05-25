namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "FollowersCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "FollowersCount", c => c.Int(nullable: false));
        }
    }
}
