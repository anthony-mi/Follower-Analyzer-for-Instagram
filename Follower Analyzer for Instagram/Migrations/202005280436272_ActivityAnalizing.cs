namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivityAnalizing : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ObservableUsers", "ApplicationUser_Id", c => c.Int());
            AddColumn("dbo.UserActivities", "ObserverPrimaryKey", c => c.String());
            AddColumn("dbo.UserActivities", "TargetUserPrimaryKey", c => c.String());
            CreateIndex("dbo.ObservableUsers", "ApplicationUser_Id");
            AddForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            DropColumn("dbo.UserActivities", "TargetUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserActivities", "TargetUserName", c => c.String());
            DropForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserActivities", "TargetUserPrimaryKey");
            DropColumn("dbo.UserActivities", "ObserverPrimaryKey");
            DropColumn("dbo.ObservableUsers", "ApplicationUser_Id");
        }
    }
}
