namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsernameMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Username");
        }
    }
}
