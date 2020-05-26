namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Username", c => c.String());
            DropColumn("dbo.Users", "FollowersCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "FollowersCount", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "Username");
        }
    }
}
