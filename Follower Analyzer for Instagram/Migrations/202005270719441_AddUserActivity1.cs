namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserActivity1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserActivities", new[] { "TargetUserPrimaryKey" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.UserActivities", "TargetUserPrimaryKey");
        }
    }
}
